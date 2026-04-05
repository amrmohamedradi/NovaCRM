using MediatR;
using NovaCRM.Application.DTOs;

namespace NovaCRM.Application.Features.Auth.Commands;
public record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResultDto>;



