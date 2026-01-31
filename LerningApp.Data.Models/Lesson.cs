using System;

namespace LerningApp.Data.Models;
public class Lesson{
    
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    
    public string Content { get; set; } = null!;
    
    public int OrderIndex { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public Guid? CourseId { get; set; } 
    public Course? Course { get; set; }
    public virtual ICollection<VocabularyItem> VocabularyItems { get; set; }
        = new HashSet<VocabularyItem>();

}