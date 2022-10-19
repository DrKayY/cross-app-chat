public class MessageService
{
    private readonly ILogger _logger;

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
    }

    public async Task<Message> AddMessage(int senderId, string message)
    {
        var createdMessage = new Message();
        using (var fStream = new FileStream("./messages.json", FileMode.Open, FileAccess.ReadWrite))
        {
            var messages = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Message>>(fStream);

            if (messages is null || messages.Count < 1)
            {
                messages = new List<Message>();
            }

            var newMessage = new Message()
            {
                Id = Guid.NewGuid(),
                Body = message,
                SenderId = senderId,
                SentAt = DateTime.Now
            };

            messages?.Add(newMessage);

            fStream.SetLength(0);
            await System.Text.Json.JsonSerializer.SerializeAsync(fStream, messages);

            createdMessage = newMessage;
        }

        return createdMessage;
    }

    public async Task<IList<Message>> GetMessages()
    {
        var allMessages = new List<Message>();
        // get sent messages
        using (var fStream = new FileStream("./messages.json", FileMode.Open, FileAccess.ReadWrite))
        {
            var messages = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Message>>(fStream);

            if (messages is null || messages.Count < 1)
            {
                messages = new List<Message>();
            }

            allMessages.AddRange(messages);
        }
        
        return allMessages;
    }

    public async Task ClearMessages()
    {
        using (var fStream = new FileStream("./messages.json", FileMode.Open, FileAccess.ReadWrite))
        {
            var messages = new List<Message>();

            fStream.SetLength(0);
            await System.Text.Json.JsonSerializer.SerializeAsync(fStream, messages);
        }
    }
}