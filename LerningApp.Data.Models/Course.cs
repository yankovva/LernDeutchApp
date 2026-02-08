using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;

public class Course
{
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; }= Guid.NewGuid();
    
    [Comment("The Name of the Course")]
    public string Name { get; set; } = null!;
    
    [Comment("The Description of the Course")]
    public string Description { get; set; } = null!;
    
    [Comment("The status of the Course")]
    public bool IsPublished { get; set; }
    
    [Comment("The Creation Date of the Course")]
    public DateTime CreatedAt { get; set; }
    
    [Comment("Foreign key to Level")]
    public Guid LevelId { get; set; } 
    
    [Comment("Level Reference")]
    public Level Level { get; set; } = null!;
    
    [Comment("Lessons in this course")]
    public virtual ICollection<Lesson> LessonsForCourse { get; set; } = new HashSet<Lesson>();
    
    [Comment("Users in this course")]
    public virtual ICollection<UserCourse> CourseParticipants { get; set; } = new HashSet<UserCourse>();
}