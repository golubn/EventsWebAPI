using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using TESTWebApi1.Models;

namespace TESTWebApi1.Context
{
    public class EventsContext : DbContext
    {
        public EventsContext(DbContextOptions<EventsContext> dbContext) : base(dbContext)
        {
            Database.EnsureCreated();
        }
        public DbSet<RegisterModel> RegisterModel { get; set; } = null!;
        
        public DbSet<Events> Events { get; set; } = null!;
    }
}
