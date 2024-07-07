using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(F1), "F1")]
[JsonDerivedType(typeof(F2), "F2")]
[JsonDerivedType(typeof(F3), "F3")]
[JsonDerivedType(typeof(F4), "F4")]
[JsonDerivedType(typeof(F5), "F5")]
[JsonDerivedType(typeof(F6), "F6")]
[JsonDerivedType(typeof(F7), "F7")]
[JsonDerivedType(typeof(F8), "F8")]
[JsonDerivedType(typeof(F9), "F9")]
public partial class QuestionBase
{
}