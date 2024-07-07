using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;

// ReSharper disable once CheckNamespace
namespace Cfo.Cats.Application.Features.Assessments.DTOs;

[JsonDerivedType(typeof(H1), "H1")]
[JsonDerivedType(typeof(H2), "H2")]
[JsonDerivedType(typeof(H3), "H3")]
[JsonDerivedType(typeof(H4), "H4")]
[JsonDerivedType(typeof(H5), "H5")]
[JsonDerivedType(typeof(H6), "H6")]
[JsonDerivedType(typeof(H7), "H7")]
[JsonDerivedType(typeof(H8), "H8")]
[JsonDerivedType(typeof(H9), "H9")]
[JsonDerivedType(typeof(H10), "H10")]
[JsonDerivedType(typeof(H11), "H11")]
[JsonDerivedType(typeof(H12), "H12")]
[JsonDerivedType(typeof(H13), "H13")]
public partial class QuestionBase
{
}

