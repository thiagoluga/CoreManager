namespace Luga.BuildingBlocks.Domain.Common;

/// <summary>
/// Resultado de uma operação: sucesso (sem valor) ou falha com <see cref="Error"/>.
/// Use <see cref="Result{T}"/> quando o sucesso carrega um valor.
/// </summary>
/// <remarks>
/// CLAUDE.md §7.15: nunca usar exception para fluxo de negócio.
/// HTTP conversion via <c>ResultExtensions.ToActionResult()</c> em <c>BuildingBlocks.Application</c>.
/// </remarks>
public class Result
{
    /// <summary>Construtor protegido — use os métodos estáticos de fábrica.</summary>
    protected Result(bool isSuccess, Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (isSuccess && error != Common.Error.None)
        {
            throw new InvalidOperationException("Sucesso não pode carregar erro.");
        }

        if (!isSuccess && error == Common.Error.None)
        {
            throw new InvalidOperationException("Falha exige um erro associado.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Sucesso? Mutuamente exclusivo com <see cref="IsFailure"/>.</summary>
    public bool IsSuccess { get; }

    /// <summary>Falha? Equivale a <c>!IsSuccess</c>.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Erro associado. <see cref="Error.None"/> quando <see cref="IsSuccess"/>.</summary>
    public Error Error { get; }

    /// <summary>Resultado de sucesso sem valor.</summary>
    public static Result Success() => new(true, Common.Error.None);

    /// <summary>Resultado de falha com erro.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Resultado de sucesso com valor.</summary>
    public static Result<T> Success<T>(T value) => Result<T>.FromValue(value);

    /// <summary>Resultado de falha tipado.</summary>
    public static Result<T> Failure<T>(Error error) => Result<T>.FromError(error);
}
