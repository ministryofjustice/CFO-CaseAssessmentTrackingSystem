using Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;

namespace Cfo.Cats.Application.Features.KeyValues.Commands.Import;

public class ImportKeyValuesCommandHandler :
    IRequestHandler<CreateKeyValueTemplateCommand, byte[]>,
    IRequestHandler<ImportKeyValuesCommand, Result>
{
    private readonly IValidator<AddEditKeyValueCommand> _addValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ImportKeyValuesCommandHandler> _localizer;
    private readonly IMapper _mapper;

    public ImportKeyValuesCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ImportKeyValuesCommandHandler> localizer,
        IValidator<AddEditKeyValueCommand> addValidator
    )
    {
         _mapper = mapper;
         _unitOfWork = unitOfWork;
         _excelService = excelService;
        _localizer = localizer;
        _addValidator = addValidator;
    }

    public async Task<byte[]> Handle(CreateKeyValueTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[]
        {
            _localizer["Name"],
            _localizer["Value"],
            _localizer["Text"],
            _localizer["Description"]
        };
        var result = await _excelService.CreateTemplateAsync(fields, _localizer["KeyValues"]);
        return result;
    }
#nullable disable warnings
    public async Task<Result> Handle(ImportKeyValuesCommand request, CancellationToken cancellationToken)
    {
        var result = await _excelService.ImportAsync(request.Data,
        new Dictionary<string, Func<DataRow, KeyValue, object?>>
        {
            {
                _localizer["Name"],
                (row, item) =>
                    item.Name = (Picklist)Enum.Parse(typeof(Picklist), row[_localizer["Name"]].ToString())
            },
            { _localizer["Value"], (row, item) => item.Value = row[_localizer["Value"]]?.ToString() },
            { _localizer["Text"], (row, item) => item.Text = row[_localizer["Text"]]?.ToString() },
            {
                _localizer["Description"],
                (row, item) => item.Description = row[_localizer["Description"]]?.ToString()
            }
        }, _localizer["Data"]);

        if (result is not { Succeeded: true, Data: not null }) return await Result.FailureAsync(result.Errors);
        {
            var importItems = result.Data;
            var errors = new List<string>();
            var errorsOccurred = false;
            foreach (var item in importItems)
            {
                var validationResult = await _addValidator.ValidateAsync(
                new AddEditKeyValueCommand
                    { Name = item.Name, Value = item.Value, Description = item.Description, Text = item.Text },
                cancellationToken);
                if (validationResult.IsValid)
                {
                    var exist = await _unitOfWork.DbContext.KeyValues.AnyAsync(x => x.Name == item.Name && x.Value == item.Value,
                    cancellationToken);
                    if (exist)
                    {
                        continue;
                    }

                    item.AddDomainEvent(new KeyValueCreatedDomainEvent(item));
                    await _unitOfWork.DbContext.KeyValues.AddAsync(item, cancellationToken);
                }
                else
                {
                    errorsOccurred = true;
                    errors.AddRange(validationResult.Errors.Select(e =>
                        $"{(!string.IsNullOrWhiteSpace(item.Name.ToString()) ? $"{item.Name} - " : string.Empty)}{e.ErrorMessage}"));
                }
            }

            if (errorsOccurred)
            {
                return await Result.FailureAsync(errors.ToArray());
            }

            
            return await Result.SuccessAsync();
        }
    }
}