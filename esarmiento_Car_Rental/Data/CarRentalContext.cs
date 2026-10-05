using esarmiento_Car_Rental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Numerics;

namespace esarmiento_Car_Rental.Data
{
    public class CarRentalContext : DbContext
    {   
        public CarRentalContext(DbContextOptions<CarRentalContext> options) 
            : base(options) 
        { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Employee> Employees { get; set; }

        //These contain all the constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany<Reservation>(e => e.Reservations) 
                .WithOne(r => r.Employee)
                .HasForeignKey(r => r.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>() //Apprently any unqiue constraint needs its own model builder
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ReservationNumber)
                .IsUnique();

            modelBuilder.Entity<MembershipType>()
                .HasMany<Customer>(m => m.Customers)
                .WithOne(c => c.MembershipType)
                .HasForeignKey(c => c.MembershipTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>() //Apprently any unqiue constraint needs its own model builder
               .HasIndex(c => c.DriverLicense)
               .IsUnique();

        }




    }
}
