using System;
using Microsoft.EntityFrameworkCore;

namespace LerningApp.Data.Models;
public class Lesson{
    
    [Comment("PK Unique Identifier")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Comment("The Name of the Lesson")]
    public string Name { get; set; } = null!;
    
    [Comment("The Target of the Lesson")]
    public string Target { get; set; } = null!;
    
    [Comment("The Content of the Lesson")]
    public string Content { get; set; } = null!;
    
    [Comment("The OrderIndex of the Lesson")]
    public int OrderIndex { get; set; }
    
    [Comment("The Creation Date of the Lesson")]
    public DateTime CreatedAt { get; set; }
    
    [Comment("Foreign key to Course")]
    public Guid? CourseId { get; set; } 
    
    [Comment("Course Reference")]
    public Course? Course { get; set; }
    public virtual ICollection<LessonSection>? LessonSections { get; set; }
        = new List<LessonSection>();
    public virtual ICollection<VocabularyCard> VocabularyCards { get; set; }
        = new HashSet<VocabularyCard>();

}