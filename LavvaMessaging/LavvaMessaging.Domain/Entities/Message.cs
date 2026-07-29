using LavvaMessaging.Domain.Enums;

namespace LavvaMessaging.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public string ExternalMessageId { get; private set; } = default!; // messageId من Infobip
    public string Content { get; private set; } = default!;
    public MessageDirection Direction { get; private set; }
    public DateTime SentAtUtc { get; private set; }
    public bool DeliveredToCrm { get; private set; }

    private Message() { } // for EF Core

    public static Message CreateInbound(Guid conversationId, string externalMessageId, string content)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            ExternalMessageId = externalMessageId,
            Content = content,
            Direction = MessageDirection.Inbound,
            SentAtUtc = DateTime.UtcNow,
            DeliveredToCrm = false
        };
    }

    public void MarkDeliveredToCrm()
    {
        DeliveredToCrm = true;
    }
}
