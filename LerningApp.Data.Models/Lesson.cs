using System;

namespace LerningApp.Data.Models;
public class Lesson{

    public Lesson()
    {
        this.Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Content { get; set; } = null!;
    
    public int OrderIndex { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public Guid CourseId { get; set; } 
    public Course Course { get; set; }
    
    public ICollection<VocabularyItem> VocabularyItems { get; set; }
        = new List<VocabularyItem>();

}