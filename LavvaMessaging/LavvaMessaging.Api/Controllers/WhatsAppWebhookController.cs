using LavvaMessaging.Application.Commands.ProcessIncomingMessage;
using LavvaMessaging.Application.DTOs;
using LavvaMessaging.Application.Interfaces;
using LavvaMessaging.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LavvaMessaging.Api.Controllers;

[ApiController]
[Route("api/webhooks/whatsapp")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebhookVerifier _verifier;
    private readonly ILogger<WhatsAppWebhookController> _logger;

    public WhatsAppWebhookController(
        IMediator mediator,
        IWebhookVerifier verifier,
        ILogger<WhatsAppWebhookController> logger)
    {
        _mediator = mediator;
        _verifier = verifier;
        _logger = logger;
    }

    // Infobip بتبعت الرسايل الجاية بـ POST مباشرة (مش زي Meta اللي محتاجة GET verification الأول)
    [HttpPost]
    public async Task<IActionResult> ReceiveMessage(CancellationToken ct)
    {
        // 1) اقرا الـ body الخام عشان لو احتجت تتحقق منه
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        // 2) تحقق من مفتاح/هيدر الأمان
        var providedToken = Request.Headers["X-Webhook-Token"].FirstOrDefault();
        if (!_verifier.Verify(providedToken, rawBody))
        {
            _logger.LogWarning("Unauthorized webhook attempt received.");
            return Unauthorized();
        }

        // 3) Parse الـ payload بتاع Infobip
        InfobipWhatsAppPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<InfobipWhatsAppPayload>(rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Infobip webhook payload.");
            return BadRequest();
        }

        if (payload?.Results is null || payload.Results.Count == 0)
        {
            return Ok(); // مفيش رسايل فعلية، رجّع 200 عادي عشان Infobip متعيدش المحاولة
        }

        // 4) حوّل كل رسالة لشكل موحّد وابعتها للمعالجة
        foreach (var result in payload.Results)
        {
            var dto = new IncomingMessageDto
            {
                Channel = MessageChannel.WhatsApp,
                CustomerExternalId = result.From,
                CustomerDisplayName = result.Contact?.Name,
                ExternalMessageId = result.MessageId,
                Content = result.Message?.Text ?? string.Empty,
                SentAtUtc = DateTimeOffset.FromUnixTimeMilliseconds(result.ReceivedAt).UtcDateTime
            };

            await _mediator.Send(new ProcessIncomingMessageCommand(dto), ct);
        }

        return Ok();
    }
}

// شكل الـ payload بتاع Infobip WhatsApp Inbound Messages
// (المرجع: Infobip WhatsApp API docs - Inbound message webhook)
public class InfobipWhatsAppPayload
{
    public List<InfobipMessageResult> Results { get; set; } = new();
}

public class InfobipMessageResult
{
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    public string MessageId { get; set; } = default!;
    public long ReceivedAt { get; set; }
    public InfobipContact? Contact { get; set; }
    public InfobipMessageContent? Message { get; set; }
}

public class InfobipContact
{
    public string? Name { get; set; }
}

public class InfobipMessageContent
{
    public string? Text { get; set; }
}
