using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class UserType
{
    [Key]
    public int UserTypeId { get; set; }

    [StringLength(100)]
    public string UserTypeName { get; set; } = null!;

    [InverseProperty("UserType")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
