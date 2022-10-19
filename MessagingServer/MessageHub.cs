using Microsoft.AspNetCore.SignalR;

public class MessageHub : Hub
{
    public async Task NotifyNewMessage(int targetAppId, Guid messageId)
    {
        await Clients.All.SendAsync($"{targetAppId}:NewChat", messageId);
    }

    public async Task NotifyDelete()
    {
        await Clients.All.SendAsync($"DeleteChats");
    }
}
