namespace Luga.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Marca a entidade como ativável/desativável (estado lógico, distinto de soft delete).
/// Transições registradas por <c>ActivationTrackingInterceptor</c>.
/// </summary>
/// <remarks>
/// Diferente de <see cref="ISoftDeletable"/>: aqui a entidade ainda é válida
/// e visível em queries, apenas operacionalmente desligada (ex.: customer
/// inativo continua existindo, mas não recebe cobranças automáticas).
/// </remarks>
public interface IActivatable
{
    /// <summary>Estado atual de ativação.</summary>
    bool IsActive { get; set; }

    /// <summary>Timestamp UTC da última ativação.</summary>
    DateTime? ActivatedOn { get; set; }

    /// <summary>Timestamp UTC da última desativação.</summary>
    DateTime? DeactivatedOn { get; set; }

    /// <summary>Motivo livre informado na última desativação.</summary>
    string? DeactivationReason { get; set; }
}
