namespace PedidosBarrio.Application.DTOs
{
    public class EmailRequestDto
    {
        public string To { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }

    public class EmailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string EmailId { get; set; }
        public DateTime SentAt { get; set; }
    }
}