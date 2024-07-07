using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(A1), nameof(A1))]
[JsonDerivedType(typeof(A2), nameof(A2))]
[JsonDerivedType(typeof(A3), nameof(A3))]
[JsonDerivedType(typeof(A4), nameof(A4))]
[JsonDerivedType(typeof(A5), nameof(A5))]
[JsonDerivedType(typeof(A6), nameof(A6))]
[JsonDerivedType(typeof(A7), nameof(A7))]
[JsonDerivedType(typeof(A8), nameof(A8))]
[JsonDerivedType(typeof(A9), nameof(A9))]
[JsonDerivedType(typeof(A10), nameof(A10))]
public partial class QuestionBase
{
}
