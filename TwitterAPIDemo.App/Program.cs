using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwitterAPIDemo.API.Services;
using TwitterAPIDemo.Core.Dtos;
using TwitterAPIDemo.Core.Events;
using TwitterAPIDemo.Core.Extensions;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();
                    configuration.AddJsonFile("appsettings.json", false, true);
                    configuration.AddJsonFile("twitterQueryParameters.json", false, true);
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddColorConsoleLogger();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ConsoleHostedService>();
                    services.AddSingleton<IConfiguration>(provider => hostContext.Configuration);
                    services.AddSingleton<IEmojiService, EmojiService>();
                    services.AddHttpClient<ITweetsService, TweetsService>();
                    services.AddHttpClient<ITwitterService, TwitterService>();
                });

        sealed class ConsoleHostedService : IHostedService
        {
            private readonly ILogger<ConsoleHostedService> _logger;
            private readonly IHostApplicationLifetime _appLifetime;
            private readonly ITwitterService _twitterService;
            private readonly ITweetsService _tweetsService;
            private readonly IEmojiService _emojiService;
            private readonly JsonSerializerOptions _options;

            public ConsoleHostedService(
                ILogger<ConsoleHostedService> logger,
                IHostApplicationLifetime appLifetime,
                IServiceProvider services)
            {
                _logger = logger;
                _appLifetime = appLifetime;
                _twitterService = services.GetService<ITwitterService>();
                _tweetsService = services.GetService<ITweetsService>();
                _emojiService = services.GetService<IEmojiService>();
                _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                _appLifetime.ApplicationStarted.Register(() =>
                {
                    Console.OutputEncoding = Encoding.UTF8;

                    EventBus<TweetReceivedEvent>.InitializeListener(@event =>
                    {
                        Task.Run(async () =>
                        {
                            Tweet newTweet;
                            try
                            {
                                var sampleTweet = JsonSerializer.Deserialize<SampleTweetDto>(@event.Message, _options);
                                newTweet = await _tweetsService.Add(sampleTweet.Data.ToTweet());
                                if ((newTweet.Id % 1000) == 0)
                                    EventBus<ReportStatsEvent>.Emit(new ReportStatsEvent(JsonSerializer.Serialize(newTweet)));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
                        }, cancellationToken);
                    });

                    EventBus<ReportStatsEvent>.InitializeListener(@event =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var statistics = await GetStatistics();
                                _logger.LogInformation(statistics);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
                        }, cancellationToken);
                    });

                    Task.Run(async () =>
                    {
                        try
                        {
                            var sampleTweets = _twitterService.GetSampleTweets();
                            await sampleTweets.ParallelForEachAsync(async sampleTweet =>
                            {
                                await Task.Run(() => EventBus<TweetReceivedEvent>.Emit(new TweetReceivedEvent(sampleTweet)));
                            }, Environment.ProcessorCount, TaskScheduler.Current);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                        finally
                        {
                            _appLifetime.StopApplication();
                        }
                    }, cancellationToken);
                });

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            private async Task<string> GetStatistics()
            {
                var emojis = await _emojiService.GetEmojis();
                var tweets = await _tweetsService.GetTweets();
                var tweetCount = tweets.Count();

                var sb = new StringBuilder("Statistics...");
                sb.AppendLine();
                sb.AppendLine();

                //Total number of tweets received
                sb.Append($"Total tweets inserted: {tweetCount}");
                sb.AppendLine();
                sb.AppendLine();

                //Average tweets per hour/minute/second
                var start = tweets.Min(t => t.CreatedDate);
                var end = tweets.Max(t => t.CreatedDate);
                var timeSpan = end.Subtract(start);
                var avePerSec = tweetCount / timeSpan.TotalSeconds;
                var avePerMin = tweetCount / timeSpan.TotalMinutes;
                var avePerHour = tweetCount / timeSpan.TotalHours;

                sb.Append("Average tweets:");
                sb.AppendLine();
                sb.Append($"{avePerHour.ToString("F")} per hour");
                sb.AppendLine();
                sb.Append($"{avePerMin.ToString("F")} per min");
                sb.AppendLine();
                sb.Append($"{avePerSec.ToString("F")} per sec");
                sb.AppendLine();
                sb.AppendLine();

                //Top emojis in tweets*
                var topEmojis = tweets
                    .SelectMany(t => t.GetEmojis(emojis))
                    .GroupBy(emoji => emoji.Key)
                    .Select(group => new { Name = emojis[group.Key], Count = group.Count() })
                    .OrderByDescending(emoji => emoji.Count)
                    .Take(10)
                    .ToList();
                sb.Append("Top 10 emojis:");
                sb.AppendLine();
                topEmojis.ForEach((e, i) => sb.Append($"{++i}. {e.Name} ({e.Count}){Environment.NewLine}"));

                //Percent of tweets that contains emojis
                var percentOfTweetsWithEmoji = Convert.ToDecimal(tweets.Count(t => t.GetEmojis(emojis).Any())) / tweetCount;
                sb.AppendLine();
                sb.AppendLine($"Percentage of tweets with emojis: {percentOfTweetsWithEmoji.ToString("P")}");
                sb.AppendLine();

                //Top hashtags
                var topHashtags = tweets
                    .SelectMany(t => t.Hashtags.Select(h => h.Tag))
                    .GroupBy(tag => tag)
                    .Select(group => new { Tag = group.Key, Count = group.Count() })
                    .OrderByDescending(tag => tag.Count)
                    .Take(10)
                    .ToList();
                sb.Append("Top 10 hashtags:");
                sb.AppendLine();
                topHashtags.ForEach((h, i) => sb.Append($"{++i}. {h.Tag} ({h.Count}){Environment.NewLine}"));

                //Percent of tweets that contain a url
                sb.AppendLine();
                var percentOfTweetsWithUrl = Convert.ToDecimal(tweets.Count(t => t.HasUrls)) / tweetCount;
                sb.AppendLine($"Percentage of tweets with url: {percentOfTweetsWithUrl.ToString("P")}");
                sb.AppendLine();

                //Percent of tweets that contain a photo url(pic.twitter.com or Instagram)
                var percentOfTweetsWithPhotoUrl = Convert.ToDecimal(tweets.Count(t => t.HasPhotoUrls)) / tweetCount;
                sb.AppendLine($"Percentage of tweets with photo url: {percentOfTweetsWithPhotoUrl.ToString("P")}");
                sb.AppendLine();

                //Top domains of urls in tweets
                var topDomainUrls = tweets
                    .SelectMany(t => t.Urls.Select(u => u.GetDomain()))
                    .GroupBy(domain => domain)
                    .Select(group => new { Name = group.Key, Count = group.Count() })
                    .OrderByDescending(domain => domain.Count)
                    .Take(10)
                    .ToList();
                sb.Append("Top 10 domains:");
                sb.AppendLine();
                topDomainUrls.ForEach((d, i) => sb.Append($"{++i}. {d.Name} ({d.Count}){Environment.NewLine}"));

                return sb.ToString();
            }
        }
    }
}
