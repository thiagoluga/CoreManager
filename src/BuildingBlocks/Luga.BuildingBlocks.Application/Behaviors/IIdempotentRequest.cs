namespace Luga.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Marks a MediatR request as eligible for idempotency-key short-circuiting.
/// Picked up by <see cref="IdempotencyBehavior{TRequest, TResponse}"/> in the pipeline.
/// </summary>
public interface IIdempotentRequest
{
    /// <summary>
    /// Client-supplied key (typically <c>Idempotency-Key</c> HTTP header) that
    /// makes a retry of the same logical operation safe.
    /// </summary>
    string IdempotencyKey { get; }
}
