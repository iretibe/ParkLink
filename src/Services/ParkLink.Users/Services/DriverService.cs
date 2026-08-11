using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkLink.SharedKernel.Pagination;
using ParkLink.Users.Data;
using ParkLink.Users.Dtos.Documents;
using ParkLink.Users.Dtos.Drivers;
using ParkLink.Users.Enums;
using ParkLink.Users.Models;

namespace ParkLink.Users.Services
{
    public class DriverService : IDriverService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public DriverService(UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task ApproveDriverAsync(string driverId, 
            DriverActionRequest? request = null, CancellationToken cancellationToken = default)
        {
            var driver = await GetDriverEntityAsync(driverId, cancellationToken);

            if (driver.DriverStatus == DriverStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The driver is already approved.");
            }

            driver.DriverStatus = DriverStatus.Approved;
            driver.IsActive = true;
            driver.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<DriverDetailsDto?> GetDriverByIdAsync(string driverId, 
            CancellationToken cancellationToken = default)
        {
            var driver = await _dbContext.Users
                .Include(x => x.Documents)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == driverId && x.IsDriver,
                    cancellationToken);

            if (driver == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(driver);

            return new DriverDetailsDto
            {
                Id = driver.Id,
                UserName = driver.UserName ?? string.Empty,
                Email = driver.Email ?? string.Empty,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                MiddleName = driver.MiddleName,
                PreferredLanguage = driver.PreferredLanguage,
                CountryCode = driver.CountryCode,
                TimeZoneId = driver.TimeZoneId,
                IsActive = driver.IsActive,
                DriverStatus = driver.DriverStatus,
                Roles = roles.ToList(),

                Documents = driver.Documents
                    .Select(x => new UserDocumentDto
                    {
                        Id = x.Id,
                        DocumentType = x.DocumentType.ToString(),
                        DocumentNumber = x.DocumentNumber,
                        IssuingCountryCode = x.IssuingCountryCode
                    })
                    .ToList()
            };
        }

        public async Task<PagedResult<DriverListItemDto>> GetDriversAsync(
            DriverSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var pageSize = request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 100);

            var query = _dbContext.Users
                .AsNoTracking()
                .Where(x => x.IsDriver);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    (x.UserName != null &&
                     x.UserName.Contains(search)) ||

                    (x.Email != null &&
                     x.Email.Contains(search)) ||

                    x.FirstName.Contains(search) ||

                    x.LastName.Contains(search) ||

                    (x.MiddleName != null &&
                     x.MiddleName.Contains(search)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.DriverStatus == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.CountryCode))
            {
                query = query.Where(x => x.CountryCode == request.CountryCode);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var drivers = await query
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = new List<DriverListItemDto>();

            foreach (var driver in drivers)
            {
                var roles = await _userManager.GetRolesAsync(driver);

                items.Add(new DriverListItemDto
                {
                    Id = driver.Id,
                    UserName = driver.UserName ?? string.Empty,
                    Email = driver.Email ?? string.Empty,
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    MiddleName = driver.MiddleName,
                    CountryCode = driver.CountryCode,
                    IsActive = driver.IsActive,
                    DriverStatus = driver.DriverStatus,
                    Roles = roles.ToList()
                });
            }

            return new PagedResult<DriverListItemDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task RejectDriverAsync(string driverId, 
            DriverActionRequest? request = null, CancellationToken cancellationToken = default)
        {
            var driver = await GetDriverEntityAsync(driverId, cancellationToken);

            if (driver.DriverStatus == DriverStatus.Approved)
            {
                throw new InvalidOperationException(
                    "An approved driver cannot be rejected. " +
                    "Suspend the driver instead.");
            }

            driver.DriverStatus = DriverStatus.Rejected;
            driver.IsActive = false;
            driver.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SuspendDriverAsync(string driverId, 
            DriverActionRequest? request = null, CancellationToken cancellationToken = default)
        {
            var driver = await GetDriverEntityAsync(driverId, cancellationToken);

            if (driver.DriverStatus != DriverStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only an approved driver can be suspended.");
            }

            driver.DriverStatus = DriverStatus.Suspended;
            driver.IsActive = false;
            driver.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<ApplicationUser> GetDriverEntityAsync(
            string driverId, CancellationToken cancellationToken)
        {
            var driver = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == driverId, cancellationToken);

            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver '{driverId}' was not found.");
            }

            return driver;
        }
    }
}
