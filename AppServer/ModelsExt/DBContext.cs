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
        
        public User? GetUser(int? id)
        {
            return this.Users.Where(u => u.UserId == id).FirstOrDefault();
        }

        public Baker? GetBaker(int? BakerId)
        {
            return this.Bakers.Where(b => b.BakerId == BakerId).Include(b => b.BakerNavigation).FirstOrDefault();
        }

        public List<Baker> GetBakers()
        {
            return this.Bakers.Include(b => b.BakerNavigation).ToList();
        }
        public List<Dessert> GetDesserts()
        {
            return this.Desserts.ToList();
        }
        public List<Order> GetOrders()
        {
            return this.Orders.ToList();
        }

        public Dessert? GetDessert(int DessertId)
        {
            return this.Desserts.Where(b => b.DessertId == DessertId).FirstOrDefault();
        }
        
        public OrderedDessert? GetOrderedDessert(int DessertId)
        {
            return this.OrderedDesserts.Where(b => b.OrderedDessertId == DessertId).FirstOrDefault();
        }

        public Order? GetOrder(int OrderId)
        {
            return this.Orders.Where(b => b.OrderId == OrderId).FirstOrDefault();
        }

    }
}
