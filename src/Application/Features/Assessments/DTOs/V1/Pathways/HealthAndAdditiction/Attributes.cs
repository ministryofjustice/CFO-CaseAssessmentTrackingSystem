using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(E1), "E1")]
[JsonDerivedType(typeof(E2), "E2")]
[JsonDerivedType(typeof(E3), "E3")]
[JsonDerivedType(typeof(E4), "E4")]
[JsonDerivedType(typeof(E5), "E5")]
[JsonDerivedType(typeof(E6), "E6")]
[JsonDerivedType(typeof(E7), "E7")]
[JsonDerivedType(typeof(E8), "E8")]
[JsonDerivedType(typeof(E9), "E9")]
public partial class QuestionBase
{
}
