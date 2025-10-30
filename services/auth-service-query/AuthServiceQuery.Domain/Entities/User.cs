﻿using System;
using System.Collections.Generic;

namespace AuthService.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public Guid? RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Oauthaccount> Oauthaccounts { get; set; } = new List<Oauthaccount>();

    public virtual UserRole? Role { get; set; }
}
