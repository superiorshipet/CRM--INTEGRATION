using LavvaMessaging.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace LavvaMessaging.Infrastructure.Infobip;

public class InfobipWebhookVerifier : IWebhookVerifier
{
    private readonly InfobipOptions _options;

    public InfobipWebhookVerifier(IOptions<InfobipOptions> options)
    {
        _options = options.Value;
    }

    public bool Verify(string? signatureOrToken, string rawBody)
    {
        // Infobip بتدعم توثيق عن طريق Basic Auth أو API Key على الـ endpoint نفسه
        // هنا بنتحقق من التوكن اللي انت حاططه في إعدادات الـ webhook بتاعتك
        if (string.IsNullOrWhiteSpace(signatureOrToken))
            return false;

        return signatureOrToken == _options.WebhookVerifyToken;
    }
}
