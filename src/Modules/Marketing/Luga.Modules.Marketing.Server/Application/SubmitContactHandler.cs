using Luga.BuildingBlocks.Domain.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Luga.Modules.Marketing.Server.Application;

/// <summary>
/// MVP handler: just logs the submission. V2 wires Mailtrap delivery + an
/// audit row in <c>marketing.lead_captures</c>.
/// </summary>
public sealed class SubmitContactHandler(ILogger<SubmitContactHandler> logger)
    : IRequestHandler<SubmitContactCommand, Result>
{
    private readonly ILogger<SubmitContactHandler> _logger = logger;

    public Task<Result> Handle(SubmitContactCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation(
            "Marketing contact form submitted: {Email} ({Name}/{Company})",
            request.Email, request.Name, request.Company ?? "(no company)");
        return Task.FromResult(Result.Success());
    }
}
