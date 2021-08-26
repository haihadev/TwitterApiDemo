using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TwitterAPIDemo.Core.Models
{
    public class Tweet : AuditEntity
    {
        public Tweet()
        {
            Hashtags = new List<Hashtag>();
            Urls = new List<Url>();
        }

        public string TwitterId { get; set; }
        public string Text { get; set; }
        public IList<Hashtag> Hashtags { get; set; }
        public IList<Url> Urls { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool HasUrls => Urls != null && Urls.Any();
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool HasPhotoUrls => Urls != null && Urls.Any(url => url.HasPhotoUrl);

        public IDictionary<string, string> GetEmojis(IDictionary<string, string> emojis)
        {
            return emojis
                .Select(e => new { Emoji = e, Found = (Text.Split(e.Key, StringSplitOptions.None).Length - 1) })
                .Where(a => a.Found > 0)
                .Select(a => a.Emoji)
                .ToDictionary(a => a.Key, a => a.Value);
        }
    }
}
