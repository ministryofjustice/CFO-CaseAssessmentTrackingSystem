using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

public class ApplyHelpLinksImportCommandHandler(
        IHelpLinkRepository repository,
        IHelpLinkCounter helpLinkCounter)
    : ICommandHandler<ApplyHelpLinksImportCommand, Result<ApplyHelpLinksImportResultDto>>
{
    public async Task<Result<ApplyHelpLinksImportResultDto>> Handle(
        ApplyHelpLinksImportCommand request,
        CancellationToken cancellationToken)
    {
        var result = new ApplyHelpLinksImportResultDto();

        // Tracks page/tab combinations created earlier in this same batch, so an import file
        // that itself contains duplicate entries can't create two new HelpLinks for the same
        // page/tab in one go.
        var createdKeysThisBatch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var decision in request.Decisions)
        {
            var urls = decision.Urls.Select(u => new HelpLinkUrlInput(u.Url, u.DisplayName));

            if (decision.ExistingHelpLinkId is { } existingId)
            {
                var existing = await repository.GetByIdAsync(existingId);
                existing.Edit(decision.Title, decision.Description, urls, decision.PageKey, decision.TabName, helpLinkCounter);
                result.Updated++;
                continue;
            }

            var key = MatchKey(decision.PageKey, decision.TabName);

            if (createdKeysThisBatch.Contains(key))
            {
                result.Skipped++;
                continue;
            }

            HelpLink helpLink;
            try
            {
                helpLink = HelpLink.Create(
                    decision.Title,
                    decision.Description,
                    urls,
                    decision.PageKey,
                    decision.TabName,
                    helpLinkCounter);
            }
            catch (BusinessRuleValidationException)
            {
                result.Skipped++;
                continue;
            }

            await repository.AddAsync(helpLink);
            createdKeysThisBatch.Add(key);
            result.Created++;
        }

        return result;
    }

    private static string MatchKey(string pageKey, string? tabName) => $"{pageKey}|{tabName}";
}
