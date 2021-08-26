using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwitterAPIDemo.Core.Extensions;

namespace TwitterAPIDemo.API.Services
{
    public class TwitterService : ITwitterService
    {
        public TwitterService(HttpClient httpClient, IConfiguration config, ILogger<TwitterService> logger)
        {
            httpClient.BaseAddress = new Uri(config["twitterApiBaseUri"]);
            httpClient.Timeout = new TimeSpan(0, 15, 0);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config["bearer"]);

            _client = httpClient;
            _config = config;
            _logger = logger;
        }

        private readonly ILogger<TwitterService> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public async Task<string> GetSingleTweet(string twitterId)
        {
            var tweetFields = _config.GetSection("tweet.fields").Get<string[]>().ToDelimitedString(field => field);
            var response = await _client.GetAsync($"{twitterId}?tweet.fields={tweetFields}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async IAsyncEnumerable<string> GetSampleTweets()
        {
            var tweetFields = _config.GetSection("tweet.fields").Get<string[]>().ToDelimitedString(field => field);
            using (var response = await _client.GetAsync($"sample/stream?tweet.fields={tweetFields}", HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Processing tweets...");

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = await reader.ReadLineAsync();
                            if (string.IsNullOrEmpty(line)) continue;
                            yield return line;
                        }
                    }
                }
            }
        }
    }
}
