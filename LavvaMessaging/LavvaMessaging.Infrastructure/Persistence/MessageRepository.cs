using LavvaMessaging.Domain.Entities;
using LavvaMessaging.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LavvaMessaging.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByExternalIdAsync(string externalMessageId, CancellationToken ct)
    {
        return await _context.Messages.AnyAsync(m => m.ExternalMessageId == externalMessageId, ct);
    }

    public async Task AddAsync(Message message, CancellationToken ct)
    {
        await _context.Messages.AddAsync(message, ct);
    }
}
