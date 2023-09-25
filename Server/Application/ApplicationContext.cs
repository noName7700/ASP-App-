using Microsoft.EntityFrameworkCore;
using Domain;

namespace Server.Application
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ActCapture> ActCapture { get; set; } = null!;
        public DbSet<Animal> animal { get; set; } = null!;
        public DbSet<Contract> Contract { get; set; } = null!;
        public DbSet<Locality> Locality { get; set; } = null!;
        public DbSet<Municipality> Municipality { get; set; } = null!;
        public DbSet<Schedule> Schedule { get; set; } = null!;
        public DbSet<TaskMonth> taskmonth { get; set; } = null!;

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
