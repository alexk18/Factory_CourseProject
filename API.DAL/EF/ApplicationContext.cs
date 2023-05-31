using API.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.EF
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Visit> Visits => Set<Visit>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Sensor> Sensors => Set<Sensor>();
        public DbSet<SensorValue> SensorValues => Set<SensorValue>();
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
