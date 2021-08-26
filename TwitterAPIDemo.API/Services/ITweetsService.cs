using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.API.Services
{
    public interface ITweetsService
    {
        Task<IList<Tweet>> GetTweets();
        Task<Tweet> Get(long id);
        Task<Tweet> Add(Tweet tweetToCreate);
        Task Update(Tweet tweetToUpdate);
        Task Delete(long id);
    }
}
