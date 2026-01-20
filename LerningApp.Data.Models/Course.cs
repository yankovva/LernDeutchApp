using System;
using System.Collections.Generic;
using System.Dynamic;

namespace LerningApp.Data.Models;

public class Course
{
    public Course()
    {
        this.Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string Level { get; set; } = null!;
    
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}