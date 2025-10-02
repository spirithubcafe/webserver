using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Application.Interfaces
{
    public interface IFooterService
    {
        Task<FooterSettings> GetFooterSettingsAsync();
        Task<FooterSettings> UpdateFooterSettingsAsync(FooterSettings settings);
        Task<List<FooterMenu>> GetFooterMenusAsync();
        Task<List<FooterMenu>> GetFooterMenusByTypeAsync(int menuType);
        Task<FooterMenu> GetFooterMenuByIdAsync(int id);
        Task<FooterMenu> CreateFooterMenuAsync(FooterMenu menu);
        Task<FooterMenu> UpdateFooterMenuAsync(FooterMenu menu);
        Task<bool> DeleteFooterMenuAsync(int id);
        Task<bool> UpdateMenuOrderAsync(List<(int id, int order)> menuOrders);
        Task<bool> ToggleMenuStatusAsync(int id);
    }
}