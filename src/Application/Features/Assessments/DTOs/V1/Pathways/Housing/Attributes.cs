using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(B1), "B1")]
[JsonDerivedType(typeof(B2), "B2")]
[JsonDerivedType(typeof(B3), "B3")]
[JsonDerivedType(typeof(B4), "B4")]
[JsonDerivedType(typeof(B5), "B5")]
[JsonDerivedType(typeof(B6), "B6")]
public partial class QuestionBase
{
}

