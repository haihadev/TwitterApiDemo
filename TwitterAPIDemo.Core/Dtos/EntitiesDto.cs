namespace TwitterAPIDemo.Core.Dtos
{
    public class EntitiesDto
    {
        public AnnotationDto[] Annotations { get; set; }
        public UrlDto[] Urls { get; set; }
        public HashtagDto[] Hashtags { get; set; }
        public MentionDto[] Mentions { get; set; }
        public CashtagDto[] Cashtags { get; set; }
    }
}
