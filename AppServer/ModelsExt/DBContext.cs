using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models
{

    public partial class DBContext : DbContext
    {
        public User? GetUser(string mail)
        {
            return this.Users.Where(u => u.Mail == mail).FirstOrDefault();
        }
    }
}
