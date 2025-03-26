using Microsoft.AspNetCore.Mvc;
using GestionFM1.Infrastructure.Data;
using GestionFM1.Infrastructure.Notification;

using System;
using Microsoft.Extensions.DependencyInjection;
using GestionFM1.Infrastructure.Data;


namespace GestionFM1.API.Controllers;
[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public TestController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpPost("test-notification")]
    public async Task<IActionResult> TestNotification()
    {
        var testNotif = new NotificationEvent {
            Id = Guid.NewGuid(),
            UserId = "05260db2-7ba8-410e-a191-baf27ebc5e17", // ID existant
            Title = "TEST DIRECT",
            Message = "Ceci est un test direct",
            CommandeId = 999,
            CreatedAt = DateTime.UtcNow
        };

        // Enregistrement direct
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        dbContext.Notifications.Add(new Notification {
            Id = testNotif.Id,
            UserId = testNotif.UserId,
            Title = testNotif.Title,
            Message = testNotif.Message,
            CommandeId = testNotif.CommandeId,
            CreatedAt = testNotif.CreatedAt,
            IsRead = false
        });
        
        await dbContext.SaveChangesAsync();

        return Ok($"Notification test {testNotif.Id} créée directement en base");
    }
}