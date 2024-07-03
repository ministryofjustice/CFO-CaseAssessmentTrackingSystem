namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
public class E4() : SingleChoiceQuestion("Are you registered with a GP and dentist?",
    [
        RegisteredWithGPAndDentist,
        RegisteredWithGPOnly,
        RegisteredWithDentistOnly,
        NotRegisteredWithGPOrDentist
    ])
{
    public const string RegisteredWithGPAndDentist = "Registered with both";
    public const string RegisteredWithGPOnly = "Registered with a GP only";
    public const string RegisteredWithDentistOnly = "Registered with a dentist only";
    public const string NotRegisteredWithGPOrDentist = "Not registered with either";

}