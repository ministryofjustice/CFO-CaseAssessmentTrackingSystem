namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record CumulativeFiguresDto(
    string contract_id,
    string description,
    int custody_enrolments,
    int community_enrolments,
    int wing_inductions,
    int hub_inductions,
    int prerelease_support,
    int ttg,
    int support_work,
    int human_citizenship,
    int community_and_social,
    int isws,
    int employment,
    int education
);