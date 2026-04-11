using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Activities.Commands;
public record DeleteActivityCommand(Guid Id) : IRequest<bool>;

public class DeleteActivityCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteActivityCommand, bool>
{
    public async Task<bool> Handle(DeleteActivityCommand request, CancellationToken ct)
    {
        var activity = await context.Activities.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Activity {request.Id} not found.");

        context.Activities.Remove(activity);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
