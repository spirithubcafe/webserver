namespace SpirithubCofe.Domain.Entities;

public class Setting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Category { get; set; } = "General";
    public string DataType { get; set; } = "Text"; // Text, Image, Boolean, Number
    public bool IsRequired { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}