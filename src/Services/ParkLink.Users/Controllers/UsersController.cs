using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.SharedKernel.Pagination;
using ParkLink.Users.Dtos.Documents;
using ParkLink.Users.Dtos.Users;
using ParkLink.Users.Services;

namespace ParkLink.Users.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Gets a paginated list of users.
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(
            [FromQuery] UserQueryParameters query, CancellationToken cancellationToken)
        {
            var result = await _userService.GetUsersAsync(query, cancellationToken);

            return Ok(result);
        }

        // Gets a user by ID.
        [HttpGet("{id}")]
        [ProducesResponseType(
        typeof(UserDto),
        StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(string id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);

            if (user == null)
            {
                return NotFound(new
                {
                    message = $"User '{id}' was not found."
                });
            }

            return Ok(user);
        }

        // Creates a new user.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> CreateUser(
            [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Unable to create user.");

                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Updates a user's profile information.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> UpdateUser(string id, 
            [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, request, cancellationToken);

                if (user == null)
                {
                    return NotFound(new
                    {
                        message = $"User '{id}' was not found."
                    });
                }

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Soft-deletes a user by setting IsActive to false.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var deleted = await _userService.DeleteUserAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"User '{id}' was not found."
                });
            }

            return NoContent();
        }

        // Activates or deactivates a user.
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserStatus(string id,
            [FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
        {
            var updated = await _userService.UpdateUserStatusAsync(id, request, cancellationToken);

            if (!updated)
            {
                return NotFound(new
                {
                    message = $"User '{id}' was not found."
                });
            }

            return NoContent();
        }

        // Replaces the user's current roles with the specified roles.
        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUserRoles(
        string id,
        [FromBody] UpdateUserRolesRequest request,
        CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _userService.UpdateUserRolesAsync(id, request, cancellationToken);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = $"User '{id}' was not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Gets all documents belonging to a user.
        [HttpGet("{userId}/documents")]
        [ProducesResponseType(typeof(IReadOnlyCollection<UserDocumentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyCollection<UserDocumentDto>>>
        GetUserDocuments(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var documents = await _userService.GetUserDocumentsAsync(userId, cancellationToken);

                return Ok(documents);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Add a document to a user.
        [HttpPost("{userId}/documents")]
        [ProducesResponseType(
        typeof(UserDocumentDto),
        StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDocumentDto>> AddUserDocument(string userId,
            [FromBody] CreateUserDocumentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _userService.AddUserDocumentAsync(userId, request, cancellationToken);

                if (document == null)
                {
                    return NotFound(new
                    {
                        message = $"User '{userId}' was not found."
                    });
                }

                return CreatedAtAction(nameof(GetUserDocuments), new { userId }, document);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // Update a user's document.
        [HttpPut("{userId}/documents/{documentId:guid}")]
        [ProducesResponseType(typeof(UserDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDocumentDto>> UpdateUserDocument(string userId, Guid documentId,
            [FromBody] UpdateUserDocumentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _userService.UpdateUserDocumentAsync(userId,
                    documentId, request, cancellationToken);

                if (document == null)
                {
                    return NotFound(new
                    {
                        message = "User document was not found."
                    });
                }

                return Ok(document);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Delete a user's document.
        [HttpDelete("{userId}/documents/{documentId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserDocument(string userId, 
            Guid documentId, CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await _userService.DeleteUserDocumentAsync(userId,
                    documentId, cancellationToken);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "User document was not found."
                    });
                }

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
