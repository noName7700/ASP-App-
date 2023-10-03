using Microsoft.EntityFrameworkCore;
using Domain;

namespace Server.Application
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ActCapture> actcapture { get; set; } = null!;
        public DbSet<Animal> animal { get; set; } = null!;
        public DbSet<Contract> contract { get; set; } = null!;
        public DbSet<Locality> locality { get; set; } = null!;
        public DbSet<Municipality_Locality> municipality_locality { get; set; } = null!;
        public DbSet<Municipality_Contract> municipality_contract { get; set; } = null!;
        public DbSet<Schedule> schedule { get; set; } = null!;
        public DbSet<TaskMonth> taskmonth { get; set; } = null!;
        public DbSet<MunicipalityName> municipalityname { get; set; } = null!;
        public DbSet<ContractNumber> contractnumber { get; set; } = null!;

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
