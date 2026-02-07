using System;
using System.Collections.Generic;
using System.Dynamic;

namespace LerningApp.Data.Models;

public class Course
{
    //TODO: Add comments everywhere and add updated at
    public Guid Id { get; set; }= Guid.NewGuid();
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public Guid LevelId { get; set; } 
    
    public Level Level { get; set; } = null!;
    
    public virtual ICollection<Lesson> LessonsForCourse { get; set; } = new HashSet<Lesson>();

    public virtual IEnumerable<UserCourse> CourseParticipants { get; set; } = new HashSet<UserCourse>();
}