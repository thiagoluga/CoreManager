namespace Luga.BuildingBlocks.Domain.Common;

/// <summary>
/// <see cref="Result"/> que carrega um valor em caso de sucesso.
/// </summary>
/// <typeparam name="T">Tipo do valor de sucesso.</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    private Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error) => _value = value;

    /// <summary>Valor de sucesso. Lança se acessado em falha.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar Value em um Result de falha.");

    /// <summary>Conversão implícita de valor → <c>Result{T}.Success</c>.</summary>
    /// <remarks>Se <c>value</c> for null, vira falha com <see cref="Error.NullValue"/>.</remarks>
    public static implicit operator Result<T>(T? value) =>
        value is not null ? FromValue(value) : FromError(Error.NullValue);

    /// <summary>Conversão implícita de erro → <c>Result{T}.Failure</c>.</summary>
    public static implicit operator Result<T>(Error error) => FromError(error);

    /// <summary>Conversão explícita (semântica clara para os analisadores).</summary>
    public static Result<T> ToResult(T value) => FromValue(value);

    /// <summary>Conversão explícita (semântica clara para os analisadores).</summary>
    public static Result<T> ToResult(Error error) => FromError(error);

    internal static Result<T> FromValue(T value) => new(value, true, Error.None);

    internal static Result<T> FromError(Error error) => new(default, false, error);
}
