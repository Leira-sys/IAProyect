namespace BlazorRecognizerWA.Models
{
    public class ResponseRecognizer
    {
        public string Type { get; set; } = string.Empty;
        public Invoice Invoice { get; set; }
        public Receipt Receipt { get; set; }
    }
}
