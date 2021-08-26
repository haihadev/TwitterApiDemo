using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterAPIDemo.Core.Models
{
    public class Url : AuditEntity
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string URL { get; set; }
        public string ExpandedUrl { get; set; }
        public string DisplayUrl { get; set; }
        public string UnwoundUrl { get; set; }

        public long TweetId { get; set; }
        public Tweet Tweet { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool HasPhotoUrl => HasTwitterUrl || HasInstagramUrl;

        private bool HasTwitterUrl => (!string.IsNullOrEmpty(DisplayUrl) && DisplayUrl.ToLower().Contains("pic.twitter.com"));
        private bool HasInstagramUrl => (!string.IsNullOrEmpty(DisplayUrl) && DisplayUrl.ToLower().Contains("instagram"));

        public string GetDomain()
        {
            var domain = string.Empty;
            Uri uri;

            if (Uri.TryCreate(ExpandedUrl, UriKind.Absolute, out uri))
            {
                domain = uri.Host;
            }
            else if (Uri.TryCreate(UnwoundUrl, UriKind.Absolute, out uri))
            {
                domain = uri.Host;
            }
            else if (Uri.TryCreate(URL, UriKind.Absolute, out uri))
            {
                domain = uri.Host;
            }
            else if (Uri.TryCreate(DisplayUrl, UriKind.Absolute, out uri))
            {
                domain = uri.Host;
            }

            return domain;
        }
    }
}
