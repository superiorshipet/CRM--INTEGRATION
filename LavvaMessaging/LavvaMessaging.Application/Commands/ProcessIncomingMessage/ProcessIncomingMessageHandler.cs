using LavvaMessaging.Application.Interfaces;
using LavvaMessaging.Domain.Entities;
using LavvaMessaging.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LavvaMessaging.Application.Commands.ProcessIncomingMessage;

public class ProcessIncomingMessageHandler : IRequestHandler<ProcessIncomingMessageCommand>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ICrmNotifier _crmNotifier;
    private readonly ILogger<ProcessIncomingMessageHandler> _logger;

    public ProcessIncomingMessageHandler(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ICrmNotifier crmNotifier,
        ILogger<ProcessIncomingMessageHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _crmNotifier = crmNotifier;
        _logger = logger;
    }

    public async Task Handle(ProcessIncomingMessageCommand request, CancellationToken ct)
    {
        var dto = request.Message;

        // 1) امنع تكرار نفس الرسالة (Idempotency) لو Infobip بعتت الـ webhook أكتر من مرة
        var alreadyExists = await _messageRepository.ExistsByExternalIdAsync(dto.ExternalMessageId, ct);
        if (alreadyExists)
        {
            _logger.LogInformation("Message {MessageId} already processed, skipping.", dto.ExternalMessageId);
            return;
        }

        // 2) دور على المحادثة أو اعمل واحدة جديدة
        var conversation = await _conversationRepository.GetByCustomerAsync(dto.CustomerExternalId, dto.Channel, ct);
        if (conversation is null)
        {
            conversation = Domain.Entities.Conversation.Create(dto.CustomerExternalId, dto.Channel, dto.CustomerDisplayName);
            await _conversationRepository.AddAsync(conversation, ct);
        }

        // 3) اعمل الرسالة واربطها بالمحادثة
        var message = Message.CreateInbound(conversation.Id, dto.ExternalMessageId, dto.Content);
        conversation.AddMessage(message);

        await _messageRepository.AddAsync(message, ct);
        await _conversationRepository.SaveChangesAsync(ct);

        // 4) ابعتها للـ CRM
        try
        {
            await _crmNotifier.NotifyNewMessageAsync(conversation, message, ct);
            message.MarkDeliveredToCrm();
            await _conversationRepository.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            // متفشلش الطلب كله لو الـ CRM واقع - سجل الخطأ وكمل
            // (ممكن تضيف Retry queue هنا بعدين)
            _logger.LogError(ex, "Failed to notify CRM for message {MessageId}", dto.ExternalMessageId);
        }
    }
}
