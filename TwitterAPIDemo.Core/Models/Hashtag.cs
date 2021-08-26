namespace TwitterAPIDemo.Core.Models
{
    public class Hashtag : AuditEntity
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Tag { get; set; }

        public long TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
