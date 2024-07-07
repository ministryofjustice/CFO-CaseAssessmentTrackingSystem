using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(G1), "G1")]
[JsonDerivedType(typeof(G2), "G2")]
[JsonDerivedType(typeof(G3), "G3")]
[JsonDerivedType(typeof(G4), "G4")]
[JsonDerivedType(typeof(G5), "G5")]
[JsonDerivedType(typeof(G6), "G6")]
[JsonDerivedType(typeof(G7), "G7")]
[JsonDerivedType(typeof(G8), "G8")]
[JsonDerivedType(typeof(G9), "G9")]
[JsonDerivedType(typeof(G10), "G10")]
public partial class QuestionBase
{
}