using LavvaMessaging.Domain.Enums;

namespace LavvaMessaging.Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; }
    public string CustomerExternalId { get; private set; } = default!; // رقم الهاتف أو Instagram Scoped ID
    public MessageChannel Channel { get; private set; }
    public string? CustomerDisplayName { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastMessageAtUtc { get; private set; }

    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    private Conversation() { } // for EF Core

    public static Conversation Create(string customerExternalId, MessageChannel channel, string? displayName)
    {
        return new Conversation
        {
            Id = Guid.NewGuid(),
            CustomerExternalId = customerExternalId,
            Channel = channel,
            CustomerDisplayName = displayName,
            CreatedAtUtc = DateTime.UtcNow,
            LastMessageAtUtc = DateTime.UtcNow
        };
    }

    public void TouchLastMessageTime()
    {
        LastMessageAtUtc = DateTime.UtcNow;
    }

    public void AddMessage(Message message)
    {
        _messages.Add(message);
        TouchLastMessageTime();
    }
}
