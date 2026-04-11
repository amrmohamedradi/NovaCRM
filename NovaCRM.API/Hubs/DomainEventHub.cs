using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NovaCRM.API.Hubs;

[Authorize]
public class DomainEventHub : Hub
{
}
