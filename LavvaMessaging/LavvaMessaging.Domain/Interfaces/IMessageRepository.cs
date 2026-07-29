using LavvaMessaging.Domain.Entities;

namespace LavvaMessaging.Domain.Interfaces;

public interface IMessageRepository
{
    Task<bool> ExistsByExternalIdAsync(string externalMessageId, CancellationToken ct);
    Task AddAsync(Message message, CancellationToken ct);
}
