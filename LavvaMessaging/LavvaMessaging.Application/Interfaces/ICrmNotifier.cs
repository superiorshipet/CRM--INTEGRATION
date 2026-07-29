using LavvaMessaging.Domain.Entities;

namespace LavvaMessaging.Application.Interfaces;

public interface ICrmNotifier
{
    Task NotifyNewMessageAsync(Conversation conversation, Message message, CancellationToken ct);
}
