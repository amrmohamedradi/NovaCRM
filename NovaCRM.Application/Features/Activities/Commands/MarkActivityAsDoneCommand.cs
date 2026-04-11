using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Activities.Commands;
public record MarkActivityAsDoneCommand(Guid Id) : IRequest<bool>;

public class MarkActivityAsDoneCommandHandler(IApplicationDbContext context)
    : IRequestHandler<MarkActivityAsDoneCommand, bool>
{
    public async Task<bool> Handle(MarkActivityAsDoneCommand request, CancellationToken ct)
    {
        var activity = await context.Activities.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Activity {request.Id} not found.");

        activity.IsDone = true;
        context.Activities.Update(activity);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
