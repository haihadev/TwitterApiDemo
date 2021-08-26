using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.API.Services
{
    public class TweetsService : ITweetsService
    {
        public TweetsService(HttpClient httpClient, IConfiguration config)
        {
            httpClient.BaseAddress = new Uri(config["tweetsApiBaseUri"]);
            httpClient.Timeout = new TimeSpan(0, 15, 0);
            _httpClient = httpClient;
            //_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReferenceHandler = ReferenceHandler.Preserve };
            _options = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }

        private readonly HttpClient _httpClient;
        //private readonly JsonSerializerOptions _options;
        private readonly JsonSerializerSettings _options;

        public async Task<IList<Tweet>> GetTweets()
        {
            var response = await _httpClient.GetAsync("api/Tweets");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            //var tweets = JsonSerializer.Deserialize<IList<Tweet>>(content, _options);
            var tweets = JsonConvert.DeserializeObject<IList<Tweet>>(content, _options);
            return tweets;
        }

        public async Task<Tweet> Get(long id)
        {
            var response = await _httpClient.GetAsync($"api/Tweets/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            //var tweet = JsonSerializer.Deserialize<Tweet>(content, _options);
            var tweet = JsonConvert.DeserializeObject<Tweet>(content, _options);
            return tweet;
        }

        public async Task<Tweet> Add(Tweet tweetToCreate)
        {
            //var tweet = JsonSerializer.Serialize(tweetToCreate);
            var tweet = JsonConvert.SerializeObject(tweetToCreate, _options);
            var requestContent = new StringContent(tweet, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Tweets", requestContent);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            //var newTweet = JsonSerializer.Deserialize<Tweet>(content, _options);
            var newTweet = JsonConvert.DeserializeObject<Tweet>(content, _options);
            return newTweet;
        }

        public async Task Update(Tweet tweetToUpdate)
        {
            //var tweet = JsonSerializer.Serialize(tweetToUpdate);
            var tweet = JsonConvert.SerializeObject(tweetToUpdate, _options);
            var requestContent = new StringContent(tweet, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Tweets/{tweetToUpdate.Id}", requestContent);

            response.EnsureSuccessStatusCode();
        }

        public async Task Delete(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/Tweets/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
