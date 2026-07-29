using LavvaMessaging.Application.Commands.ProcessIncomingMessage;
using LavvaMessaging.Application.DTOs;
using LavvaMessaging.Application.Interfaces;
using LavvaMessaging.Domain.Enums;
using LavvaMessaging.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LavvaMessaging.Api.Controllers;

[ApiController]
[Route("api/webhooks/instagram")]
public class InstagramWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebhookVerifier _verifier;
    private readonly ILogger<InstagramWebhookController> _logger;

    public InstagramWebhookController(
        IMediator mediator,
        IWebhookVerifier verifier,
        ILogger<InstagramWebhookController> logger)
    {
        _mediator = mediator;
        _verifier = verifier;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveMessage(CancellationToken ct)
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        var providedToken = Request.Headers["X-Webhook-Token"].FirstOrDefault();
        if (!_verifier.Verify(providedToken, rawBody))
        {
            _logger.LogWarning("Unauthorized webhook attempt received.");
            return Unauthorized();
        }

        InfobipWebhookPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<InfobipWebhookPayload>(rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Infobip webhook payload.");
            return BadRequest();
        }

        if (payload?.Results is null || payload.Results.Count == 0)
        {
            return Ok(); 
        }

        foreach (var result in payload.Results)
        {
            var dto = new IncomingMessageDto
            {
                Channel = MessageChannel.Instagram,
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
