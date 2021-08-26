namespace TwitterAPIDemo.Core.Events
{
    public class ReportStatsEvent
    {
        public ReportStatsEvent(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
