using System.Reflection;
using LerningApp.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LerningApp.Data;

public class LerningAppContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public LerningAppContext(DbContextOptions<LerningAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lesson> Lessons { get; set; } = null!;
    
    public virtual DbSet<Course> Courses { get; set; } = null!;
    
    public virtual DbSet<Level> Levels { get; set; } = null!;
    
    public virtual DbSet<VocabularyCard> VocabularyCards { get; set; } = null!;
    
    public virtual DbSet<VocabularyTerm> VocabularyTerms { get; set; } = null!;
    
    public virtual DbSet<PartOfSpeech> PartsOfSpeech { get; set; } = null!;



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}