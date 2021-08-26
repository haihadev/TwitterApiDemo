using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterAPIDemo.API.Services
{
    public interface ITwitterService
    {
        Task<string> GetSingleTweet(string twitterId);
        IAsyncEnumerable<string> GetSampleTweets();
    }
}
