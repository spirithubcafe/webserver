namespace SpirithubCofe.Application.DTOs;

public class FAQDto
{
    public int Id { get; set; }
    public string QuestionEn { get; set; } = "";
    public string? QuestionAr { get; set; }
    public string AnswerEn { get; set; } = "";
    public string? AnswerAr { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public FAQCategoryDto? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class FAQCategoryDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = "";
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public int FAQCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FAQPageDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = "";
    public string? TitleAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? MetaTitleEn { get; set; }
    public string? MetaTitleAr { get; set; }
    public string? MetaDescriptionEn { get; set; }
    public string? MetaDescriptionAr { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFAQDto
{
    public string QuestionEn { get; set; } = "";
    public string? QuestionAr { get; set; }
    public string AnswerEn { get; set; } = "";
    public string? AnswerAr { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CategoryId { get; set; }
}

public class UpdateFAQDto
{
    public int Id { get; set; }
    public string QuestionEn { get; set; } = "";
    public string? QuestionAr { get; set; }
    public string AnswerEn { get; set; } = "";
    public string? AnswerAr { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public int? CategoryId { get; set; }
}

public class CreateFAQCategoryDto
{
    public string NameEn { get; set; } = "";
    public string? NameAr { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateFAQCategoryDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = "";
    public string? NameAr { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
}

public class FAQPageSettingsDto
{
    public string TitleEn { get; set; } = "Frequently Asked Questions";
    public string? TitleAr { get; set; } = "الأسئلة الشائعة";
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public bool ShowSearch { get; set; } = true;
    public bool ShowCategories { get; set; } = true;
}