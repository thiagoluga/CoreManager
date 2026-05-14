namespace Luga.BuildingBlocks.Domain.Abstractions;

/// <summary>
/// Marca a entidade como soft-deletável: <c>DELETE</c> é convertido em
/// <c>UPDATE IsDeleted = 1</c> pelo <c>SoftDeleteInterceptor</c>, e linhas
/// deletadas são automaticamente filtradas via query filter global.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>Indica se a entidade foi deletada logicamente.</summary>
    bool IsDeleted { get; set; }

    /// <summary>Id do user que deletou.</summary>
    Guid? DeletedById { get; set; }

    /// <summary>Snapshot do username no momento da deleção.</summary>
    string? DeletedByUsername { get; set; }

    /// <summary>Timestamp UTC da deleção.</summary>
    DateTime? DeletedOn { get; set; }

    /// <summary>Motivo livre informado pelo user que deletou.</summary>
    string? DeletionReason { get; set; }
}
