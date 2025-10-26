using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.DTOs;
using System.Collections.Generic;

namespace AuthService.Application.Users.Queries;

public sealed class GetUsersQuery : IQuery<List<UserDto>>;