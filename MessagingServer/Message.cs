public class Message
{
    public Guid Id { get; set; }
    public int SenderId { get; set; }
    public DateTime SentAt { get; set; }
    public string? Body { get; set; }
}
