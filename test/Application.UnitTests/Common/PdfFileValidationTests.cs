using System.Text;
using Cfo.Cats.Application.Common.Validators;
using Microsoft.AspNetCore.Components.Forms;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common;

public class PdfFileValidationTests
{
    private const string ValidPdf = "%PDF-1.4\n1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\nstartxref\n9\n%%EOF\n";

    [Test]
    public async Task ValidPdf_PassesAllChecks()
    {
        var file = new FakeFile("a.pdf", ValidPdf);
        (await PdfFileValidation.BePdfFile(file, 1)).ShouldBeTrue();
        (await PdfFileValidation.HaveCompleteTrailer(file, 1)).ShouldBeTrue();
    }

    [Test]
    public async Task TruncatedPdf_FailsTrailerCheck()
    {
        var file = new FakeFile("a.pdf", ValidPdf[..30]);
        (await PdfFileValidation.BePdfFile(file, 1)).ShouldBeTrue();
        (await PdfFileValidation.HaveCompleteTrailer(file, 1)).ShouldBeFalse();
    }

    [Test]
    public async Task MissingHeader_FailsPdfCheck()
    {
        var file = new FakeFile("a.pdf", ValidPdf[10..]);
        (await PdfFileValidation.BePdfFile(file, 1)).ShouldBeFalse();
    }

    private sealed class FakeFile(string name, string content) : IBrowserFile
    {
        private readonly byte[] _bytes = Encoding.ASCII.GetBytes(content);
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;
        public long Size => _bytes.Length;
        public string ContentType => "application/pdf";
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
            => new MemoryStream(_bytes);
    }
}
