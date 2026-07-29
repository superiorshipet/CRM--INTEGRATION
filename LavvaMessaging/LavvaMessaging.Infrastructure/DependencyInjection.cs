using LavvaMessaging.Application.Interfaces;
using LavvaMessaging.Domain.Interfaces;
using LavvaMessaging.Infrastructure.Crm;
using LavvaMessaging.Infrastructure.Infobip;
using LavvaMessaging.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LavvaMessaging.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        // لو مستخدم SQL Server بدل PostgreSQL، بدّل UseNpgsql بـ UseSqlServer

        // Repositories
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // Infobip
        services.Configure<InfobipOptions>(configuration.GetSection(InfobipOptions.SectionName));
        services.AddSingleton<IWebhookVerifier, InfobipWebhookVerifier>();

        // CRM HTTP Client
        services.AddHttpClient<ICrmNotifier, CrmNotifier>(client =>
        {
            var crmBaseUrl = configuration["Crm:BaseUrl"];
            client.BaseAddress = new Uri(crmBaseUrl!);
        });

        return services;
    }
}
