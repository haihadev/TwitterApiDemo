using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TwitterAPIDemo.API.Services;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.Tests
{
    public class Tests
    {
        private IHost _client;
        private ITweetsService _tweetsService;
        private IEmojiService _emojiService;
        private ITwitterService _twitterService;
        private JsonSerializerOptions _options;

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.Sources.Clear();
                    configuration.SetBasePath(Directory.GetCurrentDirectory());
                    configuration.AddJsonFile("appsettings.json", false, true);
                    configuration.AddJsonFile("twitterQueryParameters.json", false, true);
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddLog4Net();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConfiguration>(provider => hostContext.Configuration);
                    services.AddSingleton<IEmojiService, EmojiService>();
                    services.AddHttpClient<ITweetsService, TweetsService>();
                    services.AddHttpClient<ITwitterService, TwitterService>();
                });

        [SetUp]
        public void Setup()
        {
            _client = CreateHostBuilder().Build();
            Task.Run(() => _client.RunAsync());
            _tweetsService = _client.Services.GetService<ITweetsService>();
            _emojiService = _client.Services.GetService<IEmojiService>();
            _twitterService = _client.Services.GetService<ITwitterService>();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Test]
        public async Task TestGetSingleTweet()
        {
            var tweet = await _twitterService.GetSingleTweet("1275828087666679809");
            Assert.IsTrue(!string.IsNullOrEmpty(tweet));
        }

        [Test]
        public async Task TestGetSampleTweets()
        {
            var testCount = 0;
            var sampleTweets = _twitterService.GetSampleTweets();
            // only checking a small sample to see if tweets are coming in
            await foreach (var sampleTweet in sampleTweets)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(sampleTweet));
                testCount++;
                if (testCount == 5) break;
            }
        }

        [Test]
        public async Task TestGetEmojis()
        {
            var emojis = await _emojiService.GetEmojis();
            Assert.True(emojis.Any());
        }

        [Test]
        public async Task TestGetTweets()
        {
            var tweet1 = new Tweet { TwitterId = "0", Text = "Test GetTweets() with tweet1" };
            var tweet2 = new Tweet { TwitterId = "1", Text = "Test GetTweets() with tweet2" };

            await _tweetsService.Add(tweet1);
            await _tweetsService.Add(tweet2);

            var tweets = await _tweetsService.GetTweets();
            Assert.IsTrue(tweets.Any());
        }

        [Test]
        public async Task TestGetTweet()
        {
            var tweet1 = new Tweet { TwitterId = "0", Text = "Test Get(id) tweet with tweet1" };
            var newTweet = await _tweetsService.Add(tweet1);
            var retrievedTweet = await _tweetsService.Get(newTweet.Id);
            Assert.IsNotNull(retrievedTweet.Id == newTweet.Id);
        }

        [Test]
        public async Task TestAddTweet()
        {
            var tweet = new Tweet
            {
                TwitterId = "1275828087666679809",
                Text = "Learn how to create a sentiment score for your Tweets with Microsoft Azure, Python, and Twitter Developer Labs recent search functionality.\nhttps://t.co/IKM3zo6ngu",
                Hashtags = null,
                Urls = new List<Url>
                {
                    new Url
                    {
                        Start = 140,
                        End = 163,
                        URL = "https://t.co/IKM3zo6ngu",
                        ExpandedUrl = "https://blog.twitter.com/developer/en_us/topics/tips/2020/how-to-analyze-the-sentiment-of-your-own-tweets.html",
                        DisplayUrl = "blog.twitter.com/developer/en_u…",
                        UnwoundUrl = "https://blog.twitter.com/developer/en_us/topics/tips/2020/how-to-analyze-the-sentiment-of-your-own-tweets"
                    }
                },
            };
            var newTweet = await _tweetsService.Add(tweet);
            Assert.True(newTweet.Id != 0);
        }

        [Test]
        public async Task TestUpdateTweet()
        {
            var updatedText = "Test update";
            var tweet1 = new Tweet { TwitterId = "0", Text = "Test Update(tweet) with tweet1" };
            var newTweet = await _tweetsService.Add(tweet1);
            newTweet.Text = updatedText;
            await _tweetsService.Update(newTweet);
            var updatedTweet = await _tweetsService.Get(newTweet.Id);
            Assert.IsTrue(updatedTweet.Text == updatedText);
        }

        [Test]
        public async Task TestDeleteTweet()
        {
            var tweet1 = new Tweet { TwitterId = "0", Text = "Test Delete(1) with tweet1" };
            var newTweet = await _tweetsService.Add(tweet1);
            await _tweetsService.Delete(newTweet.Id);
            var tweet = await _tweetsService.Get(newTweet.Id);
            Assert.IsNull(tweet);
        }
    }
}