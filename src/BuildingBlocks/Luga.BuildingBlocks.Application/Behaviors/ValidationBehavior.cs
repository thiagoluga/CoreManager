using FluentValidation;
using FluentValidation.Results;

using Luga.BuildingBlocks.Domain.Common;
using Luga.BuildingBlocks.Domain.Errors;

using MediatR;

namespace Luga.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Runs every registered <see cref="IValidator{T}"/> for the incoming request
/// before invoking the handler. When validation fails:
/// <list type="bullet">
/// <item>If <typeparamref name="TResponse"/> is a <see cref="Result"/> or
/// <see cref="Result{T}"/>, returns a validation failure result.</item>
/// <item>Otherwise throws a <see cref="ValidationException"/>.</item>
/// </list>
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IValidator<TRequest>[] _validators = validators?.ToArray() ?? [];

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (_validators.Length == 0)
        {
            return await next().ConfigureAwait(false);
        }

        ValidationContext<TRequest> context = new(request);

        ValidationResult[] results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)))
            .ConfigureAwait(false);

        ValidationFailure[] failures = [.. results.Where(r => !r.IsValid).SelectMany(r => r.Errors)];

        if (failures.Length == 0)
        {
            return await next().ConfigureAwait(false);
        }

        Error error = GeneralErrors.Validation(BuildMessage(failures));

        if (TryBuildFailureResult(error, out TResponse? failureResponse))
        {
            return failureResponse!;
        }

        throw new ValidationException(failures);
    }

    private static string BuildMessage(IReadOnlyList<ValidationFailure> failures) =>
        string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));

    /// <summary>
    /// When <typeparamref name="TResponse"/> derives from <see cref="Result"/>,
    /// builds a typed failure result via reflection. Returns false for any other
    /// response shape so the caller can fall back to throwing.
    /// </summary>
    private static bool TryBuildFailureResult(Error error, out TResponse? response)
    {
        response = default;
        Type responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            object failure = Result.Failure(error);
            response = (TResponse)failure;
            return true;
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = responseType.GetGenericArguments()[0];
            System.Reflection.MethodInfo? failureMethod = typeof(Result)
                .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])
                ?.MakeGenericMethod(valueType);

            if (failureMethod is null)
            {
                return false;
            }

            object? failure = failureMethod.Invoke(null, [error]);
            if (failure is null)
            {
                return false;
            }

            response = (TResponse)failure;
            return true;
        }

        return false;
    }
}
