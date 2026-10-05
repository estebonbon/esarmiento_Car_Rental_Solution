using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace esarmiento_Car_Rental.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    public DbSet<esarmiento_Car_Rental.Models.Employee> Employee { get; set; } = default!;
    }
}
