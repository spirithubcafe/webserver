using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public interface IAboutUsService
{
    Task<AboutUsPage?> GetAboutUsPageAsync();
    Task<AboutUsPage> CreateOrUpdateAboutUsPageAsync(AboutUsPage aboutUsPage);
    Task<List<AboutUsSection>> GetAboutUsSectionsAsync();
    Task<AboutUsSection?> GetAboutUsSectionByIdAsync(int id);
    Task<AboutUsSection> CreateAboutUsSectionAsync(AboutUsSection section);
    Task<AboutUsSection> UpdateAboutUsSectionAsync(AboutUsSection section);
    Task<bool> DeleteAboutUsSectionAsync(int id);
    Task<bool> ReorderSectionsAsync(List<int> sectionIds);
}