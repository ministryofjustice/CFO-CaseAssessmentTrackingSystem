namespace Cfo.Cats.Application.Common.Interfaces;

public interface IOutcomeQualityDipSampleExcelService
{
    IOutcomeQualityDipSampleExcelService WithDipSampleSummary(string region, DateTime date, string cpm, int score);
    IOutcomeQualityDipSampleExcelService AddParticipant(string participant, string type, string currentLocation, string enrolledAt, string supportWorker, bool compliant, string csoComments, string cpmComments, string finalComments);
    Task<byte[]> ExportAsync();
}
