using System;

namespace TwitterAPIDemo.Core.Dtos
{
    public class TweetDto
    {
        //attachments
        public string Author_Id { get; set; }
        //context_annotations
        public string Conversation_Id { get; set; }
        public DateTime Created_At { get; set; }
        public EntitiesDto Entities { get; set; }
        //geo
        public string Id { get; set; }
        public string In_Reply_To_User_Id { get; set; }
        public string Lang { get; set; }
        public bool Possibly_Sensitive { get; set; }
        public PublicMetricsDto Public_Metrics { get; set; }
        public ReferencedTweetDto[] Referenced_Tweets { get; set; }
        public string Reply_Settings { get; set; }
        public string Source { get; set; }
        public string Text { get; set; }
        //withheld
    }
}
