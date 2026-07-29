
namespace LavvaMessaging.Api.Models;
public class InfobipMessageResult
{
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    public string MessageId { get; set; } = default!;
    public long ReceivedAt { get; set; }
    public InfobipContact? Contact { get; set; }
    public InfobipMessageContent? Message { get; set; }
}