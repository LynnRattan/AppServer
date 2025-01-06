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

        public Baker? GetBaker(int BakerId)
        {
            return this.Bakers.Where(b => b.BakerId == BakerId).FirstOrDefault();
        }

        public List<Baker> GetBakers()
        {
            return this.Bakers.ToList();
        }
    }
}
