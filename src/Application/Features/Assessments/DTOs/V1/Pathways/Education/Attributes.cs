using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(D1), typeDiscriminator: "D1")]
[JsonDerivedType(typeof(D2), typeDiscriminator: "D2")]
[JsonDerivedType(typeof(D3), "D3")]
[JsonDerivedType(typeof(D4), "D4")]
[JsonDerivedType(typeof(D5), "D5")]
public partial class QuestionBase
{
}

