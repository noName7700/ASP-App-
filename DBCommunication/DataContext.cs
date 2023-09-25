using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classes;
using Microsoft.EntityFrameworkCore;


namespace DBCommunication
{
    public class DataContext :DbContext
    {
        public DbSet<ActCapture> ActsCaptures { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Locality> Localities { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TaskMonth> TaskMonths { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<T> DbSet<T>() where T : class
        {
            return Set<T>();
        }

        public new IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }
    }
}
