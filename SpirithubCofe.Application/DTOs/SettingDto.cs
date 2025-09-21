namespace SpirithubCofe.Application.DTOs;

public class SettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Category { get; set; } = "General";
    public string DataType { get; set; } = "Text";
    public bool IsRequired { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSettingDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Category { get; set; } = "General";
    public string DataType { get; set; } = "Text";
    public bool IsRequired { get; set; } = false;
}

public class UpdateSettingDto
{
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
}