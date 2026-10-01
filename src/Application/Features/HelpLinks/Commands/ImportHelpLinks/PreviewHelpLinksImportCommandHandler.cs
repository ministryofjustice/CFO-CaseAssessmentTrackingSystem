using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.HelpLinks;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

public class PreviewHelpLinksImportCommandHandler(IHelpLinkRepository repository)
    : ICommandHandler<PreviewHelpLinksImportCommand, Result<HelpLinkImportPreviewItemDto[]>>
{
    public async Task<Result<HelpLinkImportPreviewItemDto[]>> Handle(
        PreviewHelpLinksImportCommand request,
        CancellationToken cancellationToken)
    {
        HelpLinkExportDto[]? incomingItems;
        try
        {
            var json = System.Text.Encoding.UTF8.GetString(request.Data);
            incomingItems = JsonConvert.DeserializeObject<HelpLinkExportDto[]>(json);
        }
        catch (Newtonsoft.Json.JsonException)
        {
            return Result<HelpLinkImportPreviewItemDto[]>.Failure(
                "The uploaded file is not a valid HelpLinks export. Please check the file and try again.");
        }

        if (incomingItems is null || incomingItems.Length == 0)
        {
            return Result<HelpLinkImportPreviewItemDto[]>.Failure(
                "The uploaded file did not contain any HelpLinks to import.");
        }

        var existingHelpLinks = await repository.GetAllAsync();

        var existingByKey = existingHelpLinks
            .GroupBy(h => MatchKey(h.PageKey, h.TabName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(h => h.Created).First(),
                StringComparer.OrdinalIgnoreCase);

        var previewItems = incomingItems.Select(incoming =>
        {
            existingByKey.TryGetValue(MatchKey(incoming.PageKey, incoming.TabName), out var existing);

            var existingDto = existing is null
                ? null
                : new HelpLinkExportDto
                {
                    Title = existing.Title,
                    Description = existing.Description,
                    PageKey = existing.PageKey,
                    TabName = existing.TabName,
                    Urls = existing.Urls.Select(u => new HelpLinkUrlDto { Url = u.Url, DisplayName = u.DisplayName }).ToList()
                };

            var status = existingDto is null
                ? HelpLinkImportMatchStatus.New
                : AreEquivalent(existingDto, incoming)
                    ? HelpLinkImportMatchStatus.Unchanged
                    : HelpLinkImportMatchStatus.Conflict;

            return new HelpLinkImportPreviewItemDto
            {
                PageKey = incoming.PageKey,
                TabName = incoming.TabName,
                Incoming = incoming,
                Existing = existingDto,
                ExistingHelpLinkId = existing?.Id,
                Status = status
            };
        }).ToArray();

        return previewItems;
    }

    private static string MatchKey(string pageKey, string? tabName) =>
        $"{pageKey}|{tabName}";

    private static bool AreEquivalent(HelpLinkExportDto existing, HelpLinkExportDto incoming) =>
        existing.Title == incoming.Title
        && existing.Description == incoming.Description
        && UrlsAreEquivalent(existing.Urls, incoming.Urls);

    private static bool UrlsAreEquivalent(List<HelpLinkUrlDto> a, List<HelpLinkUrlDto> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        var normalizedA = a.Select(Normalize).OrderBy(x => x.Url, StringComparer.Ordinal).ToArray();
        var normalizedB = b.Select(Normalize).OrderBy(x => x.Url, StringComparer.Ordinal).ToArray();

        return normalizedA.SequenceEqual(normalizedB);

        static (string Url, string DisplayName) Normalize(HelpLinkUrlDto u) => (u.Url, u.DisplayName ?? string.Empty);
    }
}
