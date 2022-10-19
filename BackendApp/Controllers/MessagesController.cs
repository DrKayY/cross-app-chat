using Microsoft.AspNetCore.Mvc;

namespace BackendApp.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly MessageService _messageService;

    public MessagesController(MessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet(Name = "GetMessages")]
    public async Task<IActionResult> GetMessages()
    {
        var messages = await _messageService.GetMessages();
        return Ok(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
    {
        try
        {
            var createdMessage = await _messageService.AddMessage(sendMessageDto.SenderId, sendMessageDto.Body!);
            return Ok(createdMessage);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete(Name = "DeleteMessages")]
    public async Task<IActionResult> DeleteMessages()
    {
        await _messageService.ClearMessages();
        return Ok();
    }
}

public class SendMessageDto
{
    public int SenderId { get; set; }
    public string? Body { get; set; }
}