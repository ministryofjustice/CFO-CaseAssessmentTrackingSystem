using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(C1), "C1")]
[JsonDerivedType(typeof(C2), "C2")]
[JsonDerivedType(typeof(C3), "C3")]
[JsonDerivedType(typeof(C4), "C4")]
[JsonDerivedType(typeof(C5), "C5")]
[JsonDerivedType(typeof(C6), "C6F")]
[JsonDerivedType(typeof(C7), "C7")]
[JsonDerivedType(typeof(C8), "C8")]
[JsonDerivedType(typeof(C9), "C9")]
public partial class QuestionBase
{
}
