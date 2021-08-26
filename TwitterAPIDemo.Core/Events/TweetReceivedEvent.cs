namespace TwitterAPIDemo.Core.Events
{
    public class TweetReceivedEvent
    {
        public TweetReceivedEvent(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
