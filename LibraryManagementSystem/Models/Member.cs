using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models;

public partial class Member 
{
    public int MemberId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public DateOnly? MembershipDate { get; set; }

    public string? Status { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
