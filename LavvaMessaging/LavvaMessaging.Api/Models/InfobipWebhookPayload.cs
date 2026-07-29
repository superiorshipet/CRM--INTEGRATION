namespace LavvaMessaging.Api.Models;

public class InfobipWebhookPayload
{
    public List<InfobipMessageResult> Results { get; set; } = new();
}


