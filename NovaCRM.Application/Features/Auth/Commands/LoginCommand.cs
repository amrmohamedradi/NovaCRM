using MediatR;
using NovaCRM.Application.DTOs;

namespace NovaCRM.Application.Features.Auth.Commands;
public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;
