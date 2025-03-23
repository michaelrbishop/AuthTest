namespace AuthTest_2025_03.Models
{
    public class EmailSenderModel
    {
        public IEnumerable<string> ToEmails { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> CCEmails { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> BCCEmails { get; set; } = Enumerable.Empty<string>();
        public string Subject { get; set; } = string.Empty;
        public string MsgAsPlanText { get; set; } = string.Empty;
        public string HTMLContent { get; set; } = string.Empty;
    }
}
