using Microsoft.EntityFrameworkCore;
using TechMove.Web.Models; 
    
namespace TechMove.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Client -> Contracts (one-to-many)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contract -> ServiceRequests (one-to-many)
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision for currency fields
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostUSD)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostZAR)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.ExchangeRateUsed)
                .HasPrecision(18, 4);
        }
    }
}
