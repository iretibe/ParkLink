using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkLink.SharedKernel.Pagination;
using ParkLink.Users.Data;
using ParkLink.Users.Dtos.Documents;
using ParkLink.Users.Dtos.Users;
using ParkLink.Users.Enums;
using ParkLink.Users.Exceptions;
using ParkLink.Users.Models;

namespace ParkLink.Users.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var normalizedEmail = request.Email.Trim();

            var existingEmail = await _userManager.FindByEmailAsync(normalizedEmail);

            if (existingEmail != null)
            {
                throw new InvalidOperationException(
                    "A user with this email address already exists.");
            }

            var existingUsername = await _userManager.FindByNameAsync(request.UserName.Trim());

            if (existingUsername != null)
            {
                throw new InvalidOperationException(
                    "A user with this username already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName.Trim(),
                Email = normalizedEmail,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                MiddleName = request.MiddleName?.Trim(),
                CountryCode = request.CountryCode.Trim().ToUpperInvariant(),
                PreferredLanguage =
                    string.IsNullOrWhiteSpace(request.PreferredLanguage)
                        ? "en"
                        : request.PreferredLanguage.Trim(),
                TimeZoneId =
                    string.IsNullOrWhiteSpace(request.TimeZoneId)
                        ? "UTC"
                        : request.TimeZoneId.Trim(),
                EmailConfirmed = false,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                throw new IdentityOperationException(createResult.Errors);
            }

            if (request.Roles.Count > 0)
            {
                await ValidateRolesAsync(request.Roles, cancellationToken);

                var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    throw new IdentityOperationException(roleResult.Errors);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            return MapToDto(user, roles);
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            // Soft delete
            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new IdentityOperationException(result.Errors);
            }

            return true;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return MapToDto(user, roles);
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(
            UserQueryParameters query, CancellationToken cancellationToken = default)
        {
            var usersQuery = _dbContext.Users
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                usersQuery = usersQuery.Where(user =>
                    (user.UserName != null && user.UserName.Contains(search)) ||
                    (user.Email != null && user.Email.Contains(search)) ||
                    (user.FirstName != null && user.FirstName.Contains(search)) ||
                    (user.LastName != null && user.LastName.Contains(search)) ||
                    (user.MiddleName != null && user.MiddleName.Contains(search)));
            }

            if (query.IsActive.HasValue)
            {
                usersQuery = usersQuery.Where(
                    user => user.IsActive == query.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                usersQuery = ApplySorting(usersQuery, query.SortBy, query.SortDescending);
            }
            else
            {
                usersQuery = usersQuery
                    .OrderBy(user => user.LastName)
                    .ThenBy(user => user.FirstName);
            }

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            var users = await usersQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var userDtos = new List<UserDto>(users.Count);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(MapToDto(user, roles));
            }

            return new PagedResult<UserDto>
            {
                Items = userDtos,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserDto?> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.MiddleName = request.MiddleName?.Trim();
            user.CountryCode = request.CountryCode.Trim().ToUpperInvariant();
            user.PreferredLanguage = 
                string.IsNullOrWhiteSpace(request.PreferredLanguage)
                    ? "en"
                    : request.PreferredLanguage.Trim();
            user.TimeZoneId =
                string.IsNullOrWhiteSpace(request.TimeZoneId)
                    ? "UTC"
                    : request.TimeZoneId.Trim();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new IdentityOperationException(result.Errors);
            }

            var roles = await _userManager.GetRolesAsync(user);

            return MapToDto(user, roles);
        }

        public async Task<bool> UpdateUserRolesAsync(string id, UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var requestedRoles = request.Roles
                                    .Where(role => !string.IsNullOrWhiteSpace(role))
                                    .Select(role => role.Trim())
                                    .Distinct( StringComparer.OrdinalIgnoreCase)
                                    .ToList();

            await ValidateRolesAsync(requestedRoles, cancellationToken);

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles
                                    .Except(requestedRoles, StringComparer.OrdinalIgnoreCase)
                                    .ToList();

            var rolesToAdd = requestedRoles
                                .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
                                .ToList();

            if (rolesToRemove.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    throw new IdentityOperationException(removeResult.Errors);
                }
            }

            if (rolesToAdd.Count > 0)
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    throw new IdentityOperationException(addResult.Errors);
                }
            }

            return true;
        }

        public async Task<bool> UpdateUserStatusAsync(string id, UpdateUserStatusRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new IdentityOperationException(result.Errors);
            }

            return true;
        }

        private async Task ValidateRolesAsync(IEnumerable<string> roles, CancellationToken cancellationToken)
        {
            foreach (var role in roles)
            {
                var exists = await _roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    throw new InvalidOperationException($"The role '{role}' does not exist.");
                }
            }
        }

        private static IQueryable<ApplicationUser> ApplySorting(
            IQueryable<ApplicationUser> query, string sortBy, bool descending)
        {
            return sortBy.Trim().ToLowerInvariant() switch
            {
                "username" =>
                    descending
                        ? query.OrderByDescending(x => x.UserName)
                        : query.OrderBy(x => x.UserName),

                "email" =>
                    descending
                        ? query.OrderByDescending(x => x.Email)
                        : query.OrderBy(x => x.Email),

                "firstname" =>
                    descending
                        ? query.OrderByDescending(x => x.FirstName)
                        : query.OrderBy(x => x.FirstName),

                "lastname" =>
                    descending
                        ? query.OrderByDescending(x => x.LastName)
                        : query.OrderBy(x => x.LastName),

                "country" =>
                    descending
                        ? query.OrderByDescending(x => x.CountryCode)
                        : query.OrderBy(x => x.CountryCode),

                _ =>
                    descending
                        ? query.OrderByDescending(x => x.LastName)
                        : query.OrderBy(x => x.LastName)
            };
        }

        private static UserDto MapToDto(ApplicationUser user, IEnumerable<string> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                CountryCode = user.CountryCode,
                PreferredLanguage = user.PreferredLanguage,
                TimeZoneId = user.TimeZoneId,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                Roles = roles.ToArray()
            };
        }

        public async Task<IReadOnlyCollection<UserDocumentDto>?> GetUserDocumentsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userExists = await _dbContext.Users
                .AnyAsync(x => x.Id == userId, cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException($"User '{userId}' was not found.");
            }

            return await _dbContext.UserDocuments
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.DocumentType)
                .Select(x => new UserDocumentDto
                {
                    Id = x.Id,
                    DocumentType = x.DocumentType.ToString(),
                    DocumentNumber = x.DocumentNumber,
                    IssuingCountryCode = x.IssuingCountryCode
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<UserDocumentDto?> AddUserDocumentAsync(string userId, CreateUserDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var userExists = await _dbContext.Users
                .AnyAsync(x => x.Id == userId, cancellationToken);
            if (!userExists)
            {
                throw new KeyNotFoundException($"User '{userId}' was not found.");
            }

            // Parse string to enum for database query and assignment
            if (!Enum.TryParse<DocumentType>(request.DocumentType, ignoreCase: true, out var documentTypeEnum))
            {
                throw new ArgumentException($"Invalid document type: '{request.DocumentType}'.");
            }

            var exists = await _dbContext.UserDocuments
                .AnyAsync(x =>
                    x.IssuingCountryCode == request.IssuingCountryCode &&
                    x.DocumentType == documentTypeEnum &&
                    x.DocumentNumber == request.DocumentNumber,
                    cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("This document is already registered.");
            }

            var document = new UserDocument
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DocumentType = documentTypeEnum,
                DocumentNumber = request.DocumentNumber,
                IssuingCountryCode = request.IssuingCountryCode
            };

            _dbContext.UserDocuments.Add(document);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapDocument(document);
        }

        public async Task<UserDocumentDto?> UpdateUserDocumentAsync(string userId, Guid documentId, UpdateUserDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var document = await _dbContext.UserDocuments
                .FirstOrDefaultAsync(x => x.Id == documentId && x.UserId == userId, cancellationToken);

            if (document == null)
            {
                throw new KeyNotFoundException("User document was not found.");
            }

            if (!Enum.TryParse<DocumentType>(request.DocumentType, ignoreCase: true, out var documentTypeEnum))
            {
                throw new ArgumentException($"Invalid document type: '{request.DocumentType}'.");
            }

            document.DocumentType = documentTypeEnum;
            document.DocumentNumber = request.DocumentNumber;
            document.IssuingCountryCode = request.IssuingCountryCode;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapDocument(document);
        }

        public async Task<bool> DeleteUserDocumentAsync(string userId, Guid documentId, CancellationToken cancellationToken = default)
        {
            var document = await _dbContext.UserDocuments
                .FirstOrDefaultAsync(x => x.Id == documentId && x.UserId == userId, cancellationToken);

            if (document == null)
            {
                throw new KeyNotFoundException("User document was not found.");
            }

            _dbContext.UserDocuments.Remove(document);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static UserDocumentDto MapDocument(UserDocument document)
        {
            return new UserDocumentDto
            {
                Id = document.Id,
                DocumentType = document.DocumentType.ToString(),
                DocumentNumber = document.DocumentNumber,
                IssuingCountryCode =
                    document.IssuingCountryCode
            };
        }
    }
}
