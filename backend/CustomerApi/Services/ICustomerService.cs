using CustomerApi.DTOs;
using CustomerApi.Models;
namespace CustomerApi.Services;

public interface ICustomerService
{
    Task<object> GetCustomersAsync(
        string? name,
        string? city,
        string? country,
        bool? isActive,
        string? sortBy,
        string? sortDir,
        int pageNumber,
        int pageSize);

    Task<object?> GetCustomerByIdAsync(int id);

    Task<(bool Success, string? Error, Models.Customer? Customer)> CreateCustomerAsync(CreateCustomerDto dto);

    Task<(bool Success, string? Error)> UpdateCustomerAsync(int id, UpdateCustomerDto dto);

    Task<bool> SoftDeleteCustomerAsync(int id);

    Task<int> BulkDeactivateAsync(List<int> ids);

    Task<object> GetStatsAsync();
}