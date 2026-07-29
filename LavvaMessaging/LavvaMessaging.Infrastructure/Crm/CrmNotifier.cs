using LavvaMessaging.Application.Interfaces;
using LavvaMessaging.Domain.Entities;
using System.Net.Http.Json;

namespace LavvaMessaging.Infrastructure.Crm;

public class CrmNotifier : ICrmNotifier
{
    private readonly HttpClient _httpClient;

    public CrmNotifier(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // الـ BaseAddress متسجل في DI (هيتضاف تحت)
    }

    public async Task NotifyNewMessageAsync(Conversation conversation, Message message, CancellationToken ct)
    {
        var payload = new
        {
            conversationId = conversation.Id,
            channel = conversation.Channel.ToString(),
            customerId = conversation.CustomerExternalId,
            customerName = conversation.CustomerDisplayName,
            content = message.Content,
            sentAtUtc = message.SentAtUtc
        };

        var response = await _httpClient.PostAsJsonAsync("/api/messages/incoming", payload, ct);
        response.EnsureSuccessStatusCode();
    }
}
