using System.Reflection;
using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data;

public class LerningAppContext : DbContext
{
    public LerningAppContext(DbContextOptions options)
    :base(options)
    {
    }
    
   public virtual DbSet<Lesson> Lessons { get; set; } = null!;
   
   public virtual DbSet<Course> Courses { get; set; } = null!;

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
   }
}