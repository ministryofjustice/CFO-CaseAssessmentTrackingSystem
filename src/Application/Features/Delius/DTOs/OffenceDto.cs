using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Delius.DTOs;

public record OffenceDto(string Crn, MainOffenceDto[] MainOffences, bool IsDeleted);

public record MainOffenceDto(string OffenceDescription, DateOnly? OffenceDate, DisposalDto[] Disposals, bool IsDeleted);

public record DisposalDto(DateOnly? SentenceDate, string Length, string UnitDescription, string DisposalDetail, RequirementDto[] Requirements, string? TerminationDescription, DateOnly? TerminationDate, bool IsDeleted);

public record RequirementDto(string CategoryDescription, string SubCategoryDescription, string TerminationDescription, string Length, string UnitDescription, bool IsDeleted);
