using ClosedXML.Excel;
using Microsoft.IdentityModel.Protocols.Configuration;
using System.Runtime.CompilerServices;

namespace Cfo.Cats.Infrastructure.Services;

public class OutcomeQualityDipSampleExcelService(IOptions<DocumentExportOptions> options) : IOutcomeQualityDipSampleExcelService
{
    private (string Region, DateTime Date, string Cpm, int Score)? _summary;
    private List<(string Participant, string Type, string CurrentLocation, string EnrolledAt, string SupportWorker, string Compliant, string Feedback)> _participants = [];

    public IOutcomeQualityDipSampleExcelService AddParticipant(string participant, string type, string currentLocation, string enrolledAt, string supportWorker, bool compliant, string feedBack)
    {
        _participants.Add((participant, type, currentLocation, enrolledAt, supportWorker, compliant ? "Y" : "N", feedBack));
        return this;
    }

    public IOutcomeQualityDipSampleExcelService WithDipSampleSummary(string region, DateTime date, string cpm, int score)
    {
        _summary = (region, date, cpm, score);
        return this;
    }

    public async Task<byte[]> ExportAsync()
    {
        if(_summary == null)
        {
            throw new InvalidConfigurationException("Call to ExportAsync() before setting the sample.");
        }

        var path = Path.Combine(options.Value.TemplateDirectory, "OutcomeQualityReviewResults.xlsx");

        using var workbook = new XLWorkbook(path);

        if(workbook == null)
        {
            throw new FileNotFoundException(path);
        }

        var ws = workbook.Worksheet(1);

        // edit cells like a good un

        ws.Cell("B9").Value = _summary.Value.Region;
        ws.Cell("B10").Value = _summary.Value.Date;
        ws.Cell("B11").Value = _summary.Value.Cpm;
        ws.Cell("B12").Value = _summary.Value.Score;

        int rowIndex = 16;

        foreach (var p in _participants)
        {
            ws.Cell(rowIndex, 1).Value = p.Participant;
            ws.Cell(rowIndex, 2).Value = p.Type;
            ws.Cell(rowIndex, 3).Value = p.CurrentLocation;
            ws.Cell(rowIndex, 4).Value = p.EnrolledAt;
            ws.Cell(rowIndex, 5).Value = p.SupportWorker;
            ws.Cell(rowIndex, 6).Value = p.Compliant;
            ws.Cell(rowIndex, 7).Value = p.Feedback;

            rowIndex++;
        }

        var tableRange = ws.Range(16, 1, rowIndex, 7);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return await Task.FromResult(stream.ToArray());
    }

   
}

public class DocumentExportOptions
{
    public required string TemplateDirectory { get; set; } 
}
