using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Librarian
{

    public int LibrarianId { get; set; }

    [Required] // will have to apply data anotations leter
    public string Name { get; set; } = null!;

    [Required] // will have to apply data anotations leter
    public string Email { get; set; } = null!;

    [Required] // will have to apply data anotations leter
    public string? Role { get; set; }

    [Required] // will have to apply data anotations leter
    public DateOnly? DateJoined { get; set; }

    [Required] // will have to apply data anotations leter
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

   
}
