using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services;

public class CumulativeExcelService : ICumulativeExcelService
{
    private DateOnly? _date;
    private Actuals? _thisMonthsActuals;
    private Actuals? _lastMonthsActuals;
    private ContractTargetDto? _thisMonthsTarget;
    private ContractTargetDto? _lastMonthsTarget;

    private XLColor _backgroundColor; 

    public CumulativeExcelService(IConfiguration cfg)
    {
        _backgroundColor = XLColor.FromHtml(cfg["PrimaryColour"] ?? "#722660" );
    }

    public ICumulativeExcelService WithThisMonth(DateOnly date)
    {
        _date = date;
        return this;
    }

    public ICumulativeExcelService WithActuals(Actuals actuals)
    {
        _thisMonthsActuals = actuals;
        return this;
    }
    
    public ICumulativeExcelService WithTargets(ContractTargetDto targets)
    {
        _thisMonthsTarget = targets;
        return this;
    }
    public ICumulativeExcelService WithLastMonthActuals(Actuals actuals)
    {
        _lastMonthsActuals = actuals;
        return this;
    }
    
    public ICumulativeExcelService WithLastMonthTargets(ContractTargetDto targets)
    {
        _lastMonthsTarget = targets;
        return this;
    }

    private void SetValues(IXLWorksheet sheet, string range, XLCellValue value, int fontSize = 11, XLAlignmentHorizontalValues horizontal = XLAlignmentHorizontalValues.Center,
        XLAlignmentVerticalValues vertical = XLAlignmentVerticalValues.Center)
    {
        var cells = sheet.Range(range);
        cells.Merge();
        cells.Value = value;
        cells.Style.Font.FontSize = fontSize;
        cells.Style.Alignment.Horizontal = horizontal;
        cells.Style.Alignment.Vertical = vertical;
    }

    public Task<byte[]> ExportAsync()
    {
        Validate();

        using var workbook = new XLWorkbook();
        workbook.Properties.Author = string.Empty;

        var thisMonth = workbook.Worksheets.Add(_date!.Value.ToString("MMM yyyy"));
        SetWorksheetValues(thisMonth, _thisMonthsTarget!, _thisMonthsActuals!, _date!.Value);
        
        var lastMonth = workbook.Worksheets.Add(_date!.Value.AddMonths(-1).ToString("MMM yyyy"));
        SetWorksheetValues(lastMonth, _lastMonthsTarget!, _lastMonthsActuals!, _date!.Value.AddMonths(-1));
        
        using var stream = new MemoryStream();

        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        
        return Task.FromResult(stream.ToArray());
        
    }

    private void SetWorksheetValues(IXLWorksheet ws, ContractTargetDto targets, Actuals actuals, DateOnly date)
    {
        
        // Header Colum (the date of last month)
        SetValues(ws, "A1:M1", date!.ToString("MMM yyyy"), 16);
        
        // second row
        SetValues(ws, "A2:A4", Blank.Value, 14);
        
        SetValues(ws, "B2:E2", "Attachments", 14);
        SetValues(ws, "F2:G2", "Support & Referral", 14);
        SetValues(ws, "H2:K2", "Activities", 14);
        SetValues(ws, "L2:M2", "ETE", 14);
        
        SetValues(ws, "B3:C3", "Enrolments", 14);
        SetValues(ws, "D3:E3", "Inductions", 14);
        
        SetValues(ws, "F3:F4", "Pre-Release Support", 14);
        SetValues(ws, "G3:G4", "Through the Gate", 14);
        SetValues(ws, "H3:H4", "Support Work", 14);
        SetValues(ws, "I3:I4", "Human Citizenship", 14);
        SetValues(ws, "J3:J4", "Community and Social", 14);
        SetValues(ws, "K3:K4", "ISW Support", 14);
        SetValues(ws, "L3:L4", "Education & Training", 14);
        SetValues(ws, "M3:M4", "Employment", 14);
        SetValues(ws, "B4:B4", "Prison",12);
        SetValues(ws, "C4:C4", "Community",12);
        SetValues(ws, "D4:D4", "Wings",12);
        SetValues(ws, "E4:E4", "Hubs",12);
        
        SetValues(ws, "A5:A5", "Area Targets",12, horizontal: XLAlignmentHorizontalValues.Left);
        SetValues(ws, "A6:A6", "Actuals",12, horizontal: XLAlignmentHorizontalValues.Left);
        SetValues(ws, "A7:A7", "% Achieved",12, horizontal: XLAlignmentHorizontalValues.Left);
        
        SetValues(ws, "B5:B5", targets!.Prison);
        SetValues(ws, "C5:C5", targets!.Community);
        SetValues(ws, "D5:D5", targets!.Wings);
        SetValues(ws, "E5:E5", targets!.Hubs);
        SetValues(ws, "F5:F5", targets!.PreReleaseSupport);
        SetValues(ws, "G5:G5", targets!.ThroughTheGate);
        SetValues(ws, "H5:H5", targets!.SupportWork);
        SetValues(ws, "I5:I5", targets!.HumanCitizenship);
        SetValues(ws, "J5:J5", targets!.CommunityAndSocial);
        SetValues(ws, "K5:K5", targets!.Interventions);
        SetValues(ws, "L5:L5", targets!.TrainingAndEducation);
        SetValues(ws, "M5:M5", targets!.Employment);
        
        SetValues(ws, "B6:B6", actuals!.custody_enrolments);
        SetValues(ws, "C6:C6", actuals!.community_enrolments);
        SetValues(ws, "D6:D6", actuals!.wing_inductions);
        SetValues(ws, "E6:E6", actuals!.hub_inductions);
        SetValues(ws, "F6:F6", actuals!.prerelease_support);
        SetValues(ws, "G6:G6", actuals!.ttg);
        SetValues(ws, "H6:H6", actuals!.support_work);
        SetValues(ws, "I6:I6", actuals!.human_citizenship);
        SetValues(ws, "J6:J6", actuals!.community_and_social);
        SetValues(ws, "K6:K6", actuals!.isws);
        SetValues(ws, "L6:L6", actuals!.education);
        SetValues(ws, "M6:M6", actuals!.employment);
        
        SetValues(ws, "B7:B7", CalculatePercentage(actuals!.custody_enrolments, targets!.Prison));
        SetValues(ws, "C7:C7", CalculatePercentage(actuals!.community_enrolments, targets!.Community));
        SetValues(ws, "D7:D7", CalculatePercentage(actuals!.wing_inductions, targets!.Wings));
        SetValues(ws, "E7:E7", CalculatePercentage(actuals!.hub_inductions, targets!.Hubs));
        SetValues(ws, "F7:F7", CalculatePercentage(actuals!.prerelease_support, targets!.PreReleaseSupport));
        SetValues(ws, "G7:G7", CalculatePercentage(actuals!.ttg, targets!.ThroughTheGate));
        SetValues(ws, "H7:H7", CalculatePercentage(actuals!.support_work, targets!.SupportWork));
        SetValues(ws, "I7:I7", CalculatePercentage(actuals!.human_citizenship, targets!.HumanCitizenship));
        SetValues(ws, "J7:J7", CalculatePercentage(actuals!.community_and_social, targets!.CommunityAndSocial));
        SetValues(ws, "K7:K7", CalculatePercentage(actuals!.isws, targets!.Interventions));
        SetValues(ws, "L7:L7", CalculatePercentage(actuals!.education, targets!.TrainingAndEducation));
        SetValues(ws, "M7:M7", CalculatePercentage(actuals!.employment, targets!.Employment));

        ws.Columns("A:E").Width = 12;
        ws.Columns("F:M").Width = 15;
        ws.Range("F3:M4")
            .Style.Alignment.WrapText = true;
    
        ws.Range("A1:M4")
            .Style.Fill.BackgroundColor = _backgroundColor; 
        
        ws.Range("A1:M4")
            .Style.Font.FontColor = XLColor.White; 
        
        ws.Range("A1:M4")
            .Style.Font.Bold = true;
        
        ws.Range("A5:A7")
            .Style.Fill.BackgroundColor = _backgroundColor; 
        
        ws.Range("A5:A7")
            .Style.Font.FontColor = XLColor.White; 
        
        ws.Range("A5:A7")
            .Style.Font.Bold = true;

        for (int col = 2; col <= 13; col++)
        {
            var cell = ws.Cell(7, col);

            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            
            double value = cell.GetDouble();
            
            if (value >= 100)
            {
                cell.Style.Fill.BackgroundColor = XLColor.Green;
            }
            else if (value >= 86)
            {
                cell.Style.Fill.BackgroundColor = XLColor.Orange;
            }
            else
            {
                cell.Style.Fill.BackgroundColor = XLColor.Red;
            }
           

        }
        
        
        ws.Range("A1:M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        ws.Range("A1:M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
        ws.Range("A1:M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        ws.Range("A1:M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
        
    }

    private void Validate()
    {
        if (_date == null)
        {
            throw new ArgumentException("Date cannot be null");
        }
        if (_thisMonthsActuals == null)
        {
            throw new ArgumentException("Actuals cannot be null", nameof(_thisMonthsActuals));
        }
        if (_lastMonthsActuals == null)
        {
            throw new ArgumentException("Actuals cannot be null", nameof(_lastMonthsActuals));
        }
        if (_lastMonthsTarget == null)
        {
            throw new ArgumentException("Targets cannot be null", nameof(_lastMonthsTarget));
        }
        if (_thisMonthsTarget == null)
        {
            throw new ArgumentException("Targets cannot be null", nameof(_thisMonthsTarget));
        }
    }
    
    private static decimal CalculatePercentage(int achieved, int target)
    {
        if (target == 0)
        {
            return 0; 
        }

        decimal percentage = (decimal)achieved / target * 100;
        return Math.Round(percentage);
    }
    
}


