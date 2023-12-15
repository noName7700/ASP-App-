using Microsoft.EntityFrameworkCore;
using Domain;

namespace Server.Application
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Animal> animal { get; set; } = null!;
        public DbSet<Municipality> municipality { get; set; } = null!;
        public DbSet<Locality> locality { get; set; } = null!;
        public DbSet<Contract> contract { get; set; } = null!;
        public DbSet<ActCapture> actcapture { get; set; } = null!;
        public DbSet<Contract_Locality> contract_locality { get; set; } = null!;
        public DbSet<TaskMonth> taskmonth { get; set; } = null!;
        public DbSet<Schedule> schedule { get; set; } = null!;
        public DbSet<Usercapture> usercapture { get; set; } = null!;
        public DbSet<Organization> organization { get; set; } = null!;
        public DbSet<Journal> journal { get; set; } = null!;
        public DbSet<Role> role { get; set; } = null!;
        public DbSet<Report> report { get; set; } = null!;
        public DbSet<Status> status { get; set; } = null!;

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
