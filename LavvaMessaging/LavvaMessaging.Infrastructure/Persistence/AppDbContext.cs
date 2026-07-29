using LavvaMessaging.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LavvaMessaging.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CustomerExternalId).IsRequired().HasMaxLength(100);
            entity.HasIndex(c => new { c.CustomerExternalId, c.Channel });
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.ExternalMessageId).IsRequired().HasMaxLength(200);
            entity.HasIndex(m => m.ExternalMessageId).IsUnique();
            entity.Property(m => m.Content).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
