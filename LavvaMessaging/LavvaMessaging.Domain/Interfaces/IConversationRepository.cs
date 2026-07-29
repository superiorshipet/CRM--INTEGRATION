using LavvaMessaging.Domain.Entities;
using LavvaMessaging.Domain.Enums;

namespace LavvaMessaging.Domain.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByCustomerAsync(string customerExternalId, MessageChannel channel, CancellationToken ct);
    Task AddAsync(Conversation conversation, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
