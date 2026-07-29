namespace LavvaMessaging.Application.Interfaces;

public interface IWebhookVerifier
{
    // للتحقق من إمضاء/توكن الـ webhook قبل ما نثق في البيانات الجاية
    bool Verify(string? signatureOrToken, string rawBody);
}
