using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public interface IContactUsService
{
    Task<ContactUsPage?> GetContactUsPageAsync();
    Task<ContactUsPage> CreateOrUpdateContactUsPageAsync(ContactUsPage contactUsPage);
}