using Luga.BuildingBlocks.Domain.Common;

namespace Luga.BuildingBlocks.Domain.Errors;

/// <summary>
/// Erros reusáveis cross-module. Cada módulo terá ainda seu próprio
/// <c>{Module}Errors</c> estático com erros específicos.
/// </summary>
public static class GeneralErrors
{
    /// <summary>Entidade não encontrada por id/chave.</summary>
    public static Error NotFound(string entityName, object key) =>
        new($"{entityName}.NotFound", $"{entityName} com chave '{key}' não foi encontrado(a).");

    /// <summary>User não autenticado.</summary>
    public static Error Unauthorized() =>
        new("General.Unauthorized", "User não autenticado.");

    /// <summary>User autenticado mas sem permissão.</summary>
    public static Error Forbidden() =>
        new("General.Forbidden", "Acesso negado.");

    /// <summary>Conflito de estado (ex.: violação de unique).</summary>
    public static Error Conflict(string message) =>
        new("General.Conflict", message);

    /// <summary>Falha de validação de entrada.</summary>
    public static Error Validation(string message) =>
        new("General.Validation", message);

    /// <summary>Concorrência: outro processo atualizou a entidade primeiro.</summary>
    public static Error Concurrency() =>
        new("General.Concurrency", "A entidade foi modificada por outro processo. Recarregue e tente novamente.");

    /// <summary>Erro inesperado de infraestrutura ou bug.</summary>
    public static Error Unexpected(string message) =>
        new("General.Unexpected", message);
}
