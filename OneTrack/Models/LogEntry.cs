namespace OneTrack.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Exception { get; set; }
        public string UserName { get; set; }
        public string RequestPath { get; set; }
    }
}
