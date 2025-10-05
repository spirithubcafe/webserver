using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public interface ITermsConditionsService
{
    Task<TermsConditionsPage?> GetTermsConditionsPageAsync();
    Task<TermsConditionsPage> CreateOrUpdateTermsConditionsPageAsync(TermsConditionsPage termsConditionsPage);
    Task<List<TermsConditionsSection>> GetTermsConditionsSectionsAsync();
    Task<TermsConditionsSection?> GetTermsConditionsSectionByIdAsync(int id);
    Task<TermsConditionsSection> CreateTermsConditionsSectionAsync(TermsConditionsSection section);
    Task<TermsConditionsSection> UpdateTermsConditionsSectionAsync(TermsConditionsSection section);
    Task<bool> DeleteTermsConditionsSectionAsync(int id);
    Task<bool> ReorderSectionsAsync(List<int> sectionIds);
}
