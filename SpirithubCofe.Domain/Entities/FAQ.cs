using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public class FAQ
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string QuestionEn { get; set; } = "";
    
    [MaxLength(500)]
    public string? QuestionAr { get; set; }
    
    [Required]
    public string AnswerEn { get; set; } = "";
    
    public string? AnswerAr { get; set; }
    
    public int Order { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public int? CategoryId { get; set; }
    
    public FAQCategory? Category { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}

public class FAQCategory
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = "";
    
    [MaxLength(200)]
    public string? NameAr { get; set; }
    
    public int Order { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<FAQ> FAQs { get; set; } = new List<FAQ>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class FAQPage
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string TitleEn { get; set; } = "";
    
    [MaxLength(200)]
    public string? TitleAr { get; set; }
    
    public string? DescriptionEn { get; set; }
    
    public string? DescriptionAr { get; set; }
    
    [MaxLength(200)]
    public string? MetaTitleEn { get; set; }
    
    [MaxLength(200)]
    public string? MetaTitleAr { get; set; }
    
    public string? MetaDescriptionEn { get; set; }
    
    public string? MetaDescriptionAr { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}