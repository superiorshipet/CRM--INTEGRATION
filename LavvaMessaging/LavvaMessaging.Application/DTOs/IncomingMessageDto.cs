using LavvaMessaging.Domain.Enums;

namespace LavvaMessaging.Application.DTOs;

// شكل موحد لأي رسالة جاية من أي قناة (WhatsApp / Instagram / Messenger)
public class IncomingMessageDto
{
    public MessageChannel Channel { get; set; }
    public string CustomerExternalId { get; set; } = default!; // رقم التليفون مثلاً
    public string? CustomerDisplayName { get; set; }
    public string ExternalMessageId { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime SentAtUtc { get; set; }
}
