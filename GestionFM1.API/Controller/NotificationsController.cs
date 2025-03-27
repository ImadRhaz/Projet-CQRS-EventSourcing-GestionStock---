using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Infrastructure.Data;
using System.Collections.Generic; // Ajout de cet using
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace GestionFM1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(NotificationDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

         [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            try
            {
                _logger.LogInformation("Récupération de toutes les notifications.");

                var notifications = await _context.Notifications
                    .Where(n=> n.IsRead==false)
                    .ToListAsync();

                _logger.LogInformation($"Nombre de notifications trouvées : {notifications.Count}");

                if (notifications == null || notifications.Count == 0)
                {
                    _logger.LogWarning("Aucune notification trouvée.");
                    return NotFound("Aucune notification trouvée.");
                }

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les notifications.");
                return StatusCode(500, "Une erreur est survenue lors de la récupération des notifications.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(string userId)
        {
            try
            {
                _logger.LogInformation($"Récupération des notifications non lues pour l'utilisateur : {userId}");

                // Récupère toutes les notifications non lues pour un utilisateur donné.
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead) // Seulement les notifications non lues
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation($"Nombre de notifications non lues trouvées : {notifications.Count}");

                if (notifications == null || notifications.Count == 0)
                {
                    _logger.LogWarning($"Aucune notification non lue trouvée pour l'utilisateur : {userId}");
                    return NotFound("Aucune notification non lue trouvée pour cet utilisateur.");
                }

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération des notifications non lues pour l'utilisateur : {userId}");
                return StatusCode(500, "Une erreur est survenue lors de la récupération des notifications non lues.");
            }
        }

        [HttpPost("markAsRead/{userId}")]
        public async Task<IActionResult> MarkNotificationsAsRead(string userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                if (notifications == null || notifications.Count == 0)
                {
                    return NotFound("Aucune notification non lue trouvée pour cet utilisateur.");
                }

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();

                return Ok("Notifications marquées comme lues avec succès.");
            }
            catch (Exception ex)
            {
                // Log l'exception ici
                return StatusCode(500, "Une erreur est survenue lors de la mise à jour des notifications.");
            }
        }
        [HttpPut("markAsRead/{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    return NotFound("Notification non trouvée.");
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok("Notification marquée comme lue avec succès.");
            }
            catch (Exception ex)
            {
                // Log l'exception ici
                return StatusCode(500, "Une erreur est survenue lors de la mise à jour de la notification.");
            }
        }
    }
}