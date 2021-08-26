using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterAPIDemo.API.Services
{
    public interface IEmojiService
    {
        Task<IDictionary<string, string>> GetEmojis();
    }
}
