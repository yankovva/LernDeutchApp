using System.Reflection;
using LerningApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace LerningApp.Data;

public class LerningAppContext : DbContext
{
    public LerningAppContext(DbContextOptions<LerningAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lesson> Lessons { get; set; } = null!;
    
    public virtual DbSet<Course> Courses { get; set; } = null!;
    
    public virtual DbSet<Level> Levels { get; set; } = null!;
    
    public virtual DbSet<VocabularyItem> VocabularyItems { get; set; } = null!;
    
    public virtual DbSet<VocabularyTerm> VocabularyTerms { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}