using ParkLink.SharedKernel.Pagination;
using ParkLink.Users.Dtos.Documents;
using ParkLink.Users.Dtos.Users;

namespace ParkLink.Users.Services
{
    public interface IUserService
    {
        // Users
        Task<PagedResult<UserDto>> GetUsersAsync(UserQueryParameters query,
            CancellationToken cancellationToken = default);
        Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<UserDto> CreateUserAsync(CreateUserRequest request,
            CancellationToken cancellationToken = default);
        Task<UserDto?> UpdateUserAsync(string id, UpdateUserRequest request,
            CancellationToken cancellationToken = default);
        Task<bool> UpdateUserStatusAsync(string id, UpdateUserStatusRequest request,
            CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> UpdateUserRolesAsync(string id, UpdateUserRolesRequest request,
            CancellationToken cancellationToken = default);

        // User Documents
        Task<IReadOnlyCollection<UserDocumentDto>?> GetUserDocumentsAsync(
            string userId, CancellationToken cancellationToken = default);
        Task<UserDocumentDto?> AddUserDocumentAsync(string userId,
            CreateUserDocumentRequest request, CancellationToken cancellationToken = default);
        Task<UserDocumentDto?> UpdateUserDocumentAsync(string userId, Guid documentId,
            UpdateUserDocumentRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserDocumentAsync(string userId, Guid documentId,
            CancellationToken cancellationToken = default);
    }
}
