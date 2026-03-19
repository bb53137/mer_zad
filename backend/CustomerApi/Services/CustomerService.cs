using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetCustomersAsync(
        string? name,
        string? city,
        string? country,
        bool? isActive,
        string? sortBy,
        string? sortDir,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var query = _context.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var search = name.Trim().ToLower();
            query = query.Where(x =>
                (x.FirstName + " " + x.LastName).ToLower().Contains(search) ||
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityFilter = city.Trim().ToLower();
            query = query.Where(x => x.City.ToLower() == cityFilter);
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            var countryFilter = country.Trim().ToLower();
            query = query.Where(x => x.Country.ToLower() == countryFilter);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var sortField = sortBy?.Trim().ToLower();
        var sortDirection = sortDir?.Trim().ToLower() == "desc" ? "desc" : "asc";

        query = sortField switch
        {
            "email" => sortDirection == "desc"
                ? query.OrderByDescending(x => x.Email)
                : query.OrderBy(x => x.Email),

            "city" => sortDirection == "desc"
                ? query.OrderByDescending(x => x.City)
                : query.OrderBy(x => x.City),

            "country" => sortDirection == "desc"
                ? query.OrderByDescending(x => x.Country)
                : query.OrderBy(x => x.Country),

            "status" => sortDirection == "desc"
                ? query.OrderByDescending(x => x.IsActive)
                : query.OrderBy(x => x.IsActive),

            _ => sortDirection == "desc"
                ? query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName)
                : query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Phone,
                x.City,
                x.Country,
                x.IsActive,
                x.CreatedAt,
                x.LastModifiedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new
        {
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            items
        };
    }

    public async Task<object?> GetCustomerByIdAsync(int id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Phone,
                x.City,
                x.Country,
                x.IsActive,
                x.CreatedAt,
                x.LastModifiedAt
            })
            .FirstOrDefaultAsync();

        return customer;
    }

    public async Task<(bool Success, string? Error, Customer? Customer)> CreateCustomerAsync(CreateCustomerDto dto)
    {
        
        var normalizedEmail = dto.Email.Trim().ToLower();
        var emailExists = await _context.Customers
            .AnyAsync(x => x.Email.ToLower() == normalizedEmail);

        
        if (emailExists)
        {
            return (false, "EMAIL_EXISTS", null);
        }

        var customer = new Customer
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = normalizedEmail,
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            City = dto.City.Trim(),
            Country = dto.Country.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return (true, null, customer);
    }

    public async Task<(bool Success, string? Error)> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer == null)
        {
            return (false, "NOT_FOUND");
        }

       var normalizedEmail = dto.Email.Trim().ToLower();
       var emailExists = await _context.Customers
        .AnyAsync(x => x.Email.ToLower() == normalizedEmail && x.Id != id);
        if (emailExists)
        {
            return (false, "EMAIL_EXISTS");
        }

        customer.FirstName = dto.FirstName.Trim();
        customer.LastName = dto.LastName.Trim();
        customer.Email = normalizedEmail;
        customer.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        customer.City = dto.City.Trim();
        customer.Country = dto.Country.Trim();
        customer.IsActive = dto.IsActive;
        customer.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<bool> SoftDeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer == null)
        {
            return false;
        }

        if (!customer.IsActive)
        {
            return true;
        }

        customer.IsActive = false;
        customer.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> BulkDeactivateAsync(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return 0;

        var updatedCount = await _context.Customers
            .Where(x => ids.Contains(x.Id) && x.IsActive)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.LastModifiedAt, DateTime.UtcNow));

        return updatedCount;
    }

    public async Task<object> GetStatsAsync()
    {
        var totalCount = await _context.Customers.CountAsync();
        var activeCount = await _context.Customers.CountAsync(x => x.IsActive);
        var inactiveCount = await _context.Customers.CountAsync(x => !x.IsActive);

        var topCities = await _context.Customers
            .GroupBy(x => x.City)
            .Select(g => new
            {
                city = g.Key,
                count = g.Count()
            })
            .OrderByDescending(x => x.count)
            .Take(5)
            .ToListAsync();

        return new
        {
            totalCount,
            activeCount,
            inactiveCount,
            topCities
        };
    }
}