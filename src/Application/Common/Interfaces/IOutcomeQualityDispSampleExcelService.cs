namespace Cfo.Cats.Application.Common.Interfaces;

public interface IOutcomeQualityDispSampleExcelService
{
    IOutcomeQualityDispSampleExcelService WithDipSampleSummary(string region, DateTime date, string cpm, int score);
    IOutcomeQualityDispSampleExcelService AddParticipant(string participant, string type, string currentLocation, string enrolledAt, string supportWorker, bool compliant, string feedBack);
    Task<byte[]> ExportAsync();
}
