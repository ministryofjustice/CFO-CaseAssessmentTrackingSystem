using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Infrastructure;

internal static class PolicyExtensions
{
    /// <summary>
    /// Adds policies for CATS
    /// </summary>
    /// <param name="options"></param>
    internal static void AddCatsPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(SecurityPolicies.Export, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager);
        });

        options.AddPolicy(SecurityPolicies.CandidateSearch, policy => {
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireAuthenticatedUser();
        });

        options.AddPolicy(SecurityPolicies.DocumentUpload, policy => {
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireAuthenticatedUser();
        });

        options.AddPolicy(SecurityPolicies.AuthorizedUser, policy => {
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireAuthenticatedUser();
        });

        options.AddPolicy(SecurityPolicies.Enrol, policy => {
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireAuthenticatedUser();
        });

        options.AddPolicy(SecurityPolicies.Import, policy => {
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager);
        });

        options.AddPolicy(SecurityPolicies.SystemFunctionsRead, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QAOfficer, RoleNames.QASupportManager);
        });

        options.AddPolicy(SecurityPolicies.SystemFunctionsWrite, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QAOfficer, RoleNames.QASupportManager);
        });

        options.AddPolicy(SecurityPolicies.Pqa, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAFinance);
        });

        options.AddPolicy(SecurityPolicies.Qa1, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QAOfficer, RoleNames.QASupportManager);
        });

        options.AddPolicy(SecurityPolicies.Qa2, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QASupportManager);
        });

        options.AddPolicy(SecurityPolicies.UserHasAdditionalRoles, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager, RoleNames.QAOfficer, 
                RoleNames.QASupportManager, RoleNames.QAFinance, 
                RoleNames.PerformanceManager, RoleNames.CSO, 
                RoleNames.CPM, RoleNames.CMPSM);
        });

        options.AddPolicy(SecurityPolicies.SeniorInternal, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport, RoleNames.SMT, RoleNames.QAManager);
        });

        options.AddPolicy(SecurityPolicies.Internal, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.InternalStaff, "True");
        });

        options.AddPolicy(SecurityPolicies.ViewAudit, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport,
                RoleNames.SMT,
                RoleNames.QAManager
                );
        });

        options.AddPolicy(SecurityPolicies.SystemSupportFunctions, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport,
                RoleNames.SystemSupport,
                RoleNames.SMT
            );
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipChecks, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.CSO, RoleNames.CPM, RoleNames.CMPSM, RoleNames.SMT, RoleNames.SystemSupport);
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipReview, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.CSO, RoleNames.SMT, RoleNames.SystemSupport);
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipVerification, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.CPM, RoleNames.SMT, RoleNames.SystemSupport);
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipFinalise, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(RoleNames.CMPSM, RoleNames.SMT, RoleNames.SystemSupport);
        });
    }
}