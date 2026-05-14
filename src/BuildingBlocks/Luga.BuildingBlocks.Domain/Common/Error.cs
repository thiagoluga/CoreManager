namespace Luga.BuildingBlocks.Domain.Common;

/// <summary>
/// Erro de negócio identificado por um <see cref="Code"/> estável e legível por máquina
/// (ex.: <c>Customer.NotFound</c>) e uma <see cref="Message"/> humana.
/// </summary>
/// <remarks>
/// Convenção do <c>Code</c>: <c>{Entidade}.{Razão}</c> em PascalCase.
/// Código é o que vira HTTP problem-detail <c>type</c>; mensagem é human-readable.
/// </remarks>
public sealed record Error(string Code, string Message)
{
    /// <summary>Sentinela para indicar ausência de erro (<see cref="Result.IsSuccess"/> == true).</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Erro reservado para valor inesperadamente nulo em fluxo Result.</summary>
    public static readonly Error NullValue = new("Error.NullValue", "Valor nulo onde não era permitido.");
}
