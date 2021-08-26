using System.Linq;
using TwitterAPIDemo.Core.Dtos;
using TwitterAPIDemo.Core.Models;

namespace TwitterAPIDemo.Core.Extensions
{
    public static class DtoExtensions
    {
        public static Hashtag ToHashtag(this HashtagDto hashtagDto) =>
            new Hashtag
            {
                Start = hashtagDto.Start,
                End = hashtagDto.End,
                Tag = hashtagDto.Tag
            };

        public static Url ToUrl(this UrlDto urlDto) =>
            new Url
            {
                Start = urlDto.Start,
                End = urlDto.End,
                URL = urlDto.Url,
                ExpandedUrl = urlDto.Expanded_Url,
                DisplayUrl = urlDto.Display_Url,
                UnwoundUrl = urlDto.Unwound_Url
            };

        public static Tweet ToTweet(this TweetDto tweetDto) =>
            new Tweet
            {
                TwitterId = tweetDto.Id,
                Text = tweetDto.Text,
                Hashtags = tweetDto.Entities.Hashtags?.Select(h => h.ToHashtag()).ToList(),
                Urls = tweetDto.Entities.Urls?.Select(u => u.ToUrl()).ToList()
            };
    }
}
