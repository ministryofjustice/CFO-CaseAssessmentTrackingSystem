using System.Data;
using ClosedXML.Excel;
using Microsoft.Extensions.Localization;

namespace Cfo.Cats.Infrastructure.Services;

public class ExcelService(IStringLocalizer<ExcelService> localizer) : IExcelService
{

    public async Task<byte[]> CreateTemplateAsync(
        IEnumerable<string> fields,
        string sheetName = "Sheet1"
    )
    {
        using var workbook = new XLWorkbook();
        workbook.Properties.Author = "";
        var ws = workbook.Worksheets.Add(sheetName);
        var colIndex = 1;
        var rowIndex = 1;
        foreach (var header in fields)
        {
            var cell = ws.Cell(rowIndex, colIndex);
            var fill = cell.Style.Fill;
            fill.PatternType = XLFillPatternValues.Solid;
            fill.SetBackgroundColor(XLColor.LightBlue);
            var border = cell.Style.Border;
            border.BottomBorder =
                border.BottomBorder =
                border.BottomBorder =
                border.BottomBorder =
                    XLBorderStyleValues.Thin;

            cell.Value = header;

            colIndex++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        //var base64 = Convert.ToBase64String(stream.ToArray());
        stream.Seek(0, SeekOrigin.Begin);
        return await Task.FromResult(stream.ToArray());
    }

    public async Task<byte[]> ExportAsync<TData>(
        IEnumerable<TData> data,
        Dictionary<string, Func<TData, object?>> mappers,
        string sheetName = "Sheet1"
    )
    {
        using var workbook = new XLWorkbook();
        workbook.Properties.Author = "";
        var ws = workbook.Worksheets.Add(sheetName);
        var colIndex = 1;
        var rowIndex = 1;
        var headers = mappers.Keys.Select(x => x).ToList();
        foreach (var header in headers)
        {
            var cell = ws.Cell(rowIndex, colIndex);
            var fill = cell.Style.Fill;
            fill.PatternType = XLFillPatternValues.Solid;
            fill.SetBackgroundColor(XLColor.LightBlue);
            var border = cell.Style.Border;
            border.BottomBorder =
                border.BottomBorder =
                border.BottomBorder =
                border.BottomBorder =
                    XLBorderStyleValues.Thin;

            cell.Value = header;

            colIndex++;
        }

        var dataList = data.ToList();
        foreach (var item in dataList)
        {
            colIndex = 1;
            rowIndex++;

            var result = headers.Select(header => mappers[header](item));

            foreach (var value in result)
            {
                var cell = ws.Cell(rowIndex, colIndex++);
        
                if (value == null)
                {
                    cell.Value = Blank.Value;
                    continue;
                }

                cell.Value = value switch
                {
                    DateTime dt => dt,
                    DateOnly d => d.ToDateTime(TimeOnly.MinValue),
                    TimeOnly t => t.ToTimeSpan(),
                    DateTimeOffset dto => dto.DateTime,
                    TimeSpan ts => ts,
                    decimal d => d,
                    double db => db,
                    float f => f,
                    int i => i,
                    long l => l,
                    short s => s,
                    bool b => b,
                    _ => value.ToString()
                };

                cell.Style.NumberFormat.Format = value switch
                {
                    DateTime => "dd/mm/yyyy hh:mm:ss",
                    DateOnly => "dd/mm/yyyy",
                    TimeOnly => "hh:mm:ss",
                    DateTimeOffset => "dd/mm/yyyy hh:mm:ss",
                    TimeSpan => "hh:mm:ss",
                    _ => null
                };
            }
        }
        
        ws.ColumnsUsed().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        //var base64 = Convert.ToBase64String(stream.ToArray());
        stream.Seek(0, SeekOrigin.Begin);
        return await Task.FromResult(stream.ToArray());
    }

    public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(
        byte[] data,
        Dictionary<string, Func<DataRow, TEntity, object?>> mappers,
        string sheetName = "Sheet1"
    )
    {
        using var workbook = new XLWorkbook(new MemoryStream(data));
        if (!workbook.Worksheets.Contains(sheetName))
        {
            return Result<IEnumerable<TEntity>>.Failure(
                string.Format(localizer["Sheet with name {0} does not exist!"], sheetName)
            );
        }

        var ws = workbook.Worksheet(sheetName);
        var dt = new DataTable();
        var titlesInFirstRow = true;

        foreach (
            var firstRowCell in ws.Range(1, 1, 1, ws.LastCellUsed().Address.ColumnNumber).Cells()
        )
        {
            dt.Columns.Add(
                titlesInFirstRow
                    ? firstRowCell.GetString()
                    : $"Column {firstRowCell.Address.ColumnNumber}"
            );
        }

        var startRow = titlesInFirstRow ? 2 : 1;
        var headers = mappers.Keys.Select(x => x).ToList();
        var errors = new List<string>();
        foreach (var header in headers)
        {
            if (!dt.Columns.Contains(header))
            {
                errors.Add(
                    string.Format(localizer["Header '{0}' does not exist in table!"], header)
                );
            }
        }

        if (errors.Any())
        {
            return Result<IEnumerable<TEntity>>.Failure(errors.ToArray());
        }

        var lastRow = ws.LastRowUsed();
        var list = new List<TEntity>();
        foreach (var row in ws.Rows(startRow, lastRow!.RowNumber()))
        {
            try
            {
                var dataRow = dt.Rows.Add();
                var item =
                    (TEntity?)Activator.CreateInstance(typeof(TEntity))
                    ?? throw new NullReferenceException($"{nameof(TEntity)}");
                foreach (var cell in row.Cells())
                {
                    if (cell.DataType == XLDataType.DateTime)
                    {
                        dataRow[cell.Address.ColumnNumber - 1] = cell.GetDateTime()
                            .ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        dataRow[cell.Address.ColumnNumber - 1] = cell.Value.ToString();
                    }
                }

                headers.ForEach(x => mappers[x](dataRow, item));
                list.Add(item);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<TEntity>>.Failure(
                    string.Format(localizer["Sheet name {0}:{1}"], sheetName, e.Message)
                );
            }
        }
        await Task.CompletedTask;
        return Result<IEnumerable<TEntity>>.Success(list);
    }
}
