using LavvaMessaging.Application.DTOs;
using MediatR;

namespace LavvaMessaging.Application.Commands.ProcessIncomingMessage;

public class ProcessIncomingMessageCommand : IRequest
{
    public IncomingMessageDto Message { get; }

    public ProcessIncomingMessageCommand(IncomingMessageDto message)
    {
        Message = message;
    }
}
