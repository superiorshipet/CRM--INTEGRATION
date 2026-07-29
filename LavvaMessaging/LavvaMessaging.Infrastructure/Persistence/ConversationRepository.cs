using LavvaMessaging.Domain.Entities;
using LavvaMessaging.Domain.Enums;
using LavvaMessaging.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LavvaMessaging.Infrastructure.Persistence;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _context;

    public ConversationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByCustomerAsync(string customerExternalId, MessageChannel channel, CancellationToken ct)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.CustomerExternalId == customerExternalId && c.Channel == channel, ct);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken ct)
    {
        await _context.Conversations.AddAsync(conversation, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
