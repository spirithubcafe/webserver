using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public interface IPrivacyPolicyService
{
    Task<PrivacyPolicyPage?> GetPrivacyPolicyPageAsync();
    Task<PrivacyPolicyPage> CreateOrUpdatePrivacyPolicyPageAsync(PrivacyPolicyPage privacyPolicyPage);
    Task<List<PrivacyPolicySection>> GetPrivacyPolicySectionsAsync();
    Task<PrivacyPolicySection?> GetPrivacyPolicySectionByIdAsync(int id);
    Task<PrivacyPolicySection> CreatePrivacyPolicySectionAsync(PrivacyPolicySection section);
    Task<PrivacyPolicySection> UpdatePrivacyPolicySectionAsync(PrivacyPolicySection section);
    Task<bool> DeletePrivacyPolicySectionAsync(int id);
    Task<bool> ReorderSectionsAsync(List<int> sectionIds);
}
