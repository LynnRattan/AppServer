using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Baker> Bakers { get; set; }

    public virtual DbSet<ConfectioneryType> ConfectioneryTypes { get; set; }

    public virtual DbSet<Dessert> Desserts { get; set; }

    public virtual DbSet<DessertType> DessertTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderedDessert> OrderedDesserts { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=AppServer_DB;User ID=AdminLogin;Password=12345;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Baker>(entity =>
        {
            entity.HasKey(e => e.BakerId).HasName("PK__Bakers__51A73EC6ED5FE463");

            entity.Property(e => e.BakerId).ValueGeneratedNever();

            entity.HasOne(d => d.BakerNavigation).WithOne(p => p.Baker)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bakers__BakerId__300424B4");

            entity.HasOne(d => d.ConfectioneryType).WithMany(p => p.Bakers).HasConstraintName("FK__Bakers__Confecti__30F848ED");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Bakers).HasConstraintName("FK__Bakers__StatusCo__31EC6D26");
        });

        modelBuilder.Entity<ConfectioneryType>(entity =>
        {
            entity.HasKey(e => e.ConfectioneryTypeId).HasName("PK__Confecti__D3EF7D35E55CC591");

            entity.Property(e => e.ConfectioneryTypeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Dessert>(entity =>
        {
            entity.HasKey(e => e.DessertId).HasName("PK__Desserts__2FF5148BC3E831C1");

            entity.HasOne(d => d.Baker).WithMany(p => p.Desserts).HasConstraintName("FK__Desserts__BakerI__34C8D9D1");

            entity.HasOne(d => d.DessertType).WithMany(p => p.Desserts).HasConstraintName("FK__Desserts__Desser__35BCFE0A");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Desserts).HasConstraintName("FK__Desserts__Status__36B12243");
        });

        modelBuilder.Entity<DessertType>(entity =>
        {
            entity.HasKey(e => e.DessertTypeId).HasName("PK__DessertT__891B64BAE901BB43");

            entity.Property(e => e.DessertTypeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF3C53667C");

            entity.HasOne(d => d.Baker).WithMany(p => p.Orders).HasConstraintName("FK__Orders__BakerId__3B75D760");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK__Orders__Customer__3A81B327");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Orders).HasConstraintName("FK__Orders__StatusCo__398D8EEE");
        });

        modelBuilder.Entity<OrderedDessert>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.DessertId }).HasName("PK_Orders_Desserts");

            entity.HasOne(d => d.Dessert).WithMany(p => p.OrderedDesserts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderedDe__Desse__3F466844");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedDesserts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderedDe__Order__3E52440B");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.OrderedDesserts).HasConstraintName("FK__OrderedDe__Statu__403A8C7D");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusCode).HasName("PK__Statuses__6A7B44FDBE2315B7");

            entity.Property(e => e.StatusCode).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C7E06C544");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users).HasConstraintName("FK__Users__UserTypeI__2D27B809");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.UserTypeId).HasName("PK__UserType__40D2D816AF25F400");

            entity.Property(e => e.UserTypeId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
