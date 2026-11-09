using System.Text;
using FluentValidation;
using Humanizer;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Common.Validators;

// Shared checks for uploaded PDF files, used by the upload validators and the upload preview.
public static class PdfFileValidation
{
    // PDFs end with "startxref", an offset and "%%EOF". A small window allows for trailing whitespace or padding.
    private const int TrailerWindowBytes = 2048;

    public const string NotAPdfMessage = "File is not a valid PDF";
    public const string DamagedPdfMessage = "This PDF appears to be damaged or incomplete. Please check the file and try again";

    // Checks size, signature and trailer
    public static IRuleBuilderOptionsConditions<T, IBrowserFile?> MustBeValidPdf<T>(this IRuleBuilder<T, IBrowserFile?> ruleBuilder, double maxSizeMB)
        => ruleBuilder.CustomAsync(async (file, context, cancellationToken) =>
        {
            if (file is null)
            {
                return;
            }

            if (!NotExceedMaximumFileSize(file, maxSizeMB))
            {
                context.AddFailure($"This file is too large ({file.Size.Bytes().Humanize("0.#")}). The maximum allowed size is {maxSizeMB} MB.");
            }
            else if (!await BePdfFile(file, maxSizeMB, cancellationToken))
            {
                context.AddFailure(NotAPdfMessage);
            }
            else if (!await HaveCompleteTrailer(file, maxSizeMB, cancellationToken))
            {
                context.AddFailure(DamagedPdfMessage);
            }
        });

    public static bool NotExceedMaximumFileSize(IBrowserFile? file, double maxSizeMB)
        => file?.Size < ByteSize.FromMegabytes(maxSizeMB).Bytes;

    // Checks the extension, MIME type and %PDF signature
    public static async Task<bool> BePdfFile(IBrowserFile? file, double maxSizeMB, CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return false;
        }

        if (!Path.GetExtension(file.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (file.ContentType != "application/pdf")
        {
            return false;
        }

        await using var stream = file.OpenReadStream(MaxBytes(maxSizeMB), cancellationToken);
        var buffer = new byte[4];
        await stream.ReadExactlyAsync(buffer.AsMemory(0, 4), cancellationToken);
        return Encoding.ASCII.GetString(buffer) == "%PDF";
    }

    // Catches truncated files only; damage elsewhere in the file is not detected
    public static async Task<bool> HaveCompleteTrailer(IBrowserFile? file, double maxSizeMB, CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return false;
        }

        await using var stream = file.OpenReadStream(MaxBytes(maxSizeMB), cancellationToken);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        var bytes = memoryStream.GetBuffer().AsMemory(0, (int)memoryStream.Length).Span;
        var tail = bytes.Length > TrailerWindowBytes ? bytes[^TrailerWindowBytes..] : bytes;
        var text = Encoding.ASCII.GetString(tail);

        return text.Contains("startxref", StringComparison.Ordinal)
               && text.Contains("%%EOF", StringComparison.Ordinal);
    }

    private static long MaxBytes(double maxSizeMB) => Convert.ToInt64(ByteSize.FromMegabytes(maxSizeMB).Bytes);
}
