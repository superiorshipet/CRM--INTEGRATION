namespace LavvaMessaging.Infrastructure.Infobip;

public class InfobipOptions
{
    public const string SectionName = "Infobip";
    public string ApiKey { get; set; } = default!;
    public string BaseUrl { get; set; } = default!;
    public string WebhookVerifyToken { get; set; } = default!; // انت بتحدده في Infobip Portal
}
