using Microsoft.AspNetCore.Mvc;
using ParkLink.Notification.Dtos;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Pagination;
using System.Security.Claims;

namespace ParkLink.Notification.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<NotificationListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<NotificationListItemDto>>>
            GetNotifications([FromQuery] NotificationSearchRequest request,
                CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            var result = await _notificationService.GetNotificationsAsync(
                userId, request, cancellationToken
            );

            return Ok(result);
        }

        [HttpGet("unread")]
        [ProducesResponseType(typeof(PagedResult<NotificationListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<NotificationListItemDto>>>
            GetUnreadNotifications([FromQuery] NotificationSearchRequest request, CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            var result = await _notificationService.GetUnreadNotificationsAsync(
                userId, request, cancellationToken
            );

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(NotificationDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationDetailsDto>>
            GetNotification(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            var notification = await _notificationService.GetNotificationByIdAsync(
                id, userId, cancellationToken
            );

            if (notification is null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        [HttpPost("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            await _notificationService.MarkAsReadAsync(id, userId, cancellationToken);

            return NoContent();
        }

        [HttpPost("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            await _notificationService.DeleteNotificationAsync(id, userId, cancellationToken);

            return NoContent();
        }

        [HttpGet("statistics")]
        [ProducesResponseType(typeof(NotificationStatisticsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationStatisticsDto>>
            GetStatistics(CancellationToken cancellationToken)
        {
            var userId = GetRequiredUserId();

            var result = await _notificationService.GetStatisticsAsync(userId, cancellationToken);

            return Ok(result);
        }

        private string GetRequiredUserId()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "Authenticated user ID was not found.");
            }

            return userId;
        }
    }
}
