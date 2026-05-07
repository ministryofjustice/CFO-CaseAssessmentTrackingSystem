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
        options.AddPolicy(SecurityPolicies.AuthorizedUser, policy => {
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
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.PQA);
        });

        options.AddPolicy(SecurityPolicies.Qa1, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.QA1);
        });

        options.AddPolicy(SecurityPolicies.UserManagement, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.UserManagement);
        });

        options.AddPolicy(SecurityPolicies.Qa2, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.QA2);
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
            policy.RequireClaim(ApplicationClaimTypes.Permission, "Senior Internal");
        });

        options.AddPolicy(SecurityPolicies.Internal, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.InternalStaff, "True");
        });

        options.AddPolicy(SecurityPolicies.SystemSupportFunctions, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireRole(RoleNames.SystemSupport);
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipChecks, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission , "Outcome Quality Dip Checks");
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipReview, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission , "Outcome Quality Dip Review");
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipVerification, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, "Outcome Quality Dip Verification");
            policy.RequireRole(RoleNames.CPM, RoleNames.SMT, RoleNames.SystemSupport);
        });

        options.AddPolicy(SecurityPolicies.OutcomeQualityDipFinalise, policy => {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, "Outcome Quality Dip Finalise");
        });

        options.AddPolicy(SecurityPolicies.ContractData, policy =>
        {
           policy.RequireAuthenticatedUser();
           policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
           policy.RequireClaim(ApplicationClaimTypes.Permission, "Contract Data"); 
        });
        
        options.AddPolicy(SecurityPolicies.Reassign, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, "Reassign");
        });

        options.AddPolicy(SecurityPolicies.Transfers, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, "Transfers");
        });
        
        options.AddPolicy(SecurityPolicies.Finance, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, "Finance");       
        });

        options.AddPolicy(SecurityPolicies.ServiceDeskManagement, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.ServiceDeskManagement);  

        });

        options.AddPolicy(SecurityPolicies.Initiatives, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.Initiatives);
        });

        options.AddPolicy(SecurityPolicies.ManageInitiatives, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ApplicationClaimTypes.AccountLocked, "False");
            policy.RequireClaim(ApplicationClaimTypes.Permission, Permissions.ManageInitiatives);
        });
        
    }
}