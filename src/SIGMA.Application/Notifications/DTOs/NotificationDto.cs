using SIGMA.Domain.Enums;

namespace SIGMA.Application.Notifications.DTOs;

// DTO de notificacion expuesto al frontend, con los datos necesarios para listar y marcar como leida
public class NotificationDto
{
    public Guid Id { get; init; }
    public NotificationType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Guid? RelatedEntityId { get; init; }
    public bool IsRead { get; init; }
    public DateTime? ReadAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
