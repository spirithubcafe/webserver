using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public interface IRefundPolicyService
{
    Task<RefundPolicyPage?> GetRefundPolicyPageAsync();
    Task<RefundPolicyPage> CreateOrUpdateRefundPolicyPageAsync(RefundPolicyPage refundPolicyPage);
    Task<List<RefundPolicySection>> GetRefundPolicySectionsAsync();
    Task<RefundPolicySection?> GetRefundPolicySectionByIdAsync(int id);
    Task<RefundPolicySection> CreateRefundPolicySectionAsync(RefundPolicySection section);
    Task<RefundPolicySection> UpdateRefundPolicySectionAsync(RefundPolicySection section);
    Task<bool> DeleteRefundPolicySectionAsync(int id);
    Task<bool> ReorderSectionsAsync(List<int> sectionIds);
}
