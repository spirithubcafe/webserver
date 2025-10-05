using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public interface IDeliveryPolicyService
{
    Task<DeliveryPolicyPage?> GetDeliveryPolicyPageAsync();
    Task<DeliveryPolicyPage> CreateOrUpdateDeliveryPolicyPageAsync(DeliveryPolicyPage deliveryPolicyPage);
    Task<List<DeliveryPolicySection>> GetDeliveryPolicySectionsAsync();
    Task<DeliveryPolicySection?> GetDeliveryPolicySectionByIdAsync(int id);
    Task<DeliveryPolicySection> CreateDeliveryPolicySectionAsync(DeliveryPolicySection section);
    Task<DeliveryPolicySection> UpdateDeliveryPolicySectionAsync(DeliveryPolicySection section);
    Task<bool> DeleteDeliveryPolicySectionAsync(int id);
    Task<bool> ReorderSectionsAsync(List<int> sectionIds);
}
