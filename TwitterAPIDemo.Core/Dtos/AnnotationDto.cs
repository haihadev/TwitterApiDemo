namespace TwitterAPIDemo.Core.Dtos
{
    public class AnnotationDto
    {
        public int Start { get; set; }
        public int End { get; set; }
        public decimal Probability { get; set; }
        public string Type { get; set; }
        public string Normalized_Text { get; set; }
    }
}
