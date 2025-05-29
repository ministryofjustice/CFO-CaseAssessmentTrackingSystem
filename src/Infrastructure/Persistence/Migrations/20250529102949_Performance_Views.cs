using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Performance_Views : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER FUNCTION mi.GetCumulativeTotals(
                                        @StartDate DATE,
                                        @EndDate DATE
                                    )
                                    RETURNS TABLE
                                    AS
                                    RETURN
                                    (
                                        WITH payment_summary AS (
                                            SELECT 
                                                c.id AS contract_id,
                                                c.Description as [Description],
                                                COALESCE(Custody.eligible_count, 0) AS custody_enrolments,
                                                COALESCE(Community.eligible_count, 0) AS community_enrolments,
                                                COALESCE(Wing.eligible_count, 0) as wing_inductions,
                                                COALESCE(Hub.eligible_count, 0) as hub_inductions,
                                                COALESCE(prs.eligible_count, 0) as prerelease_support,
                                                COALESCE(ttg.eligible_count, 0) as ttg,
                                                COALESCE(support_work.eligible_count, 0) as support_work,
                                                COALESCE(human_citizenship.eligible_count, 0) as human_citizenship,
                                                COALESCE(community_social.eligible_count, 0) as community_and_social,
                                                COALESCE(isws.eligible_count, 0) as isws,
                                                COALESCE(employment.eligible_count, 0) as employment,
                                                COALESCE(education.eligible_count, 0) as education
                                                
                                            FROM [Configuration].[Contract] as c
                                            LEFT JOIN (
                                                SELECT 
                                                    ContractId,
                                                    COUNT(*) AS eligible_count
                                                FROM mi.EnrolmentPayment
                                                WHERE EligibleForPayment = 1  
                                                  AND Approved BETWEEN @StartDate AND @EndDate  
                                                   AND LocationType IN ('Wing',
                                                                      'Feeder',
                                                                      'Outlying',
                                                                      'Female',
                                                                      'Unmapped Custody')
                                                GROUP BY ContractId
                                            ) as Custody ON c.Id = Custody.ContractId
                                            LEFT JOIN (
                                                SELECT 
                                                    ContractId,
                                                    COUNT(*) AS eligible_count
                                                FROM mi.EnrolmentPayment
                                                WHERE EligibleForPayment = 1  
                                                  AND Approved BETWEEN @StartDate AND @EndDate  
                                                  AND LocationType IN ('Community',
                                                                      'Hub',
                                                                      'Satellite',
                                                                      'Unmapped Community')
                                                GROUP BY ContractId
                                            ) as Community ON c.Id = Community.ContractId
                                            LEFT JOIN (
                                                SELECT 
                                                    ContractId,
                                                    COUNT(*) AS eligible_count
                                                FROM mi.InductionPayment
                                                WHERE EligibleForPayment = 1  
                                                  AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
                                                  AND LocationType IN ('Wing')
                                                GROUP BY ContractId
                                            ) as Wing ON c.Id = Wing.ContractId
                                            LEFT JOIN (
                                                SELECT 
                                                    ContractId,
                                                    COUNT(*) AS eligible_count
                                                FROM mi.InductionPayment
                                                WHERE EligibleForPayment = 1  
                                                  AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
                                                  AND LocationType IN ('Hub')
                                                GROUP BY ContractId
                                            ) as Hub ON c.Id = Hub.ContractId
        LEFT JOIN (
            SELECT 
                ContractId,
                COUNT(*) AS eligible_count
            FROM mi.SupportAndReferralPayment
            WHERE EligibleForPayment = 1  
              AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
              AND SupportType IN ('Pre-Release Support')
            GROUP BY ContractId
        ) as prs ON c.Id = prs.ContractId
        LEFT JOIN (
            SELECT 
                ContractId,
                COUNT(*) AS eligible_count
            FROM mi.SupportAndReferralPayment
            WHERE EligibleForPayment = 1  
              AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
              AND SupportType IN ('Through the Gate')
            GROUP BY ContractId
        ) as ttg ON c.Id = ttg.ContractId
        LEFT JOIN (
            SELECT 
                ContractId,
                COUNT(*) AS eligible_count
            FROM mi.ActivityPayment
            WHERE EligibleForPayment = 1  
              AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
              AND ActivityType IN ('Support Work')
            GROUP BY ContractId
        ) as support_work ON c.Id = support_work.ContractId
        LEFT JOIN (
            SELECT 
                ContractId,
                COUNT(*) AS eligible_count
            FROM mi.ActivityPayment
            WHERE EligibleForPayment = 1  
              AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
              AND ActivityType IN ('Human Citizenship')
            GROUP BY ContractId
        ) as human_citizenship ON c.Id = human_citizenship.ContractId
        LEFT JOIN (
            SELECT 
                ContractId,
                COUNT(*) AS eligible_count
            FROM mi.ActivityPayment
            WHERE EligibleForPayment = 1  
              AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
              AND ActivityType IN ('Community and Social')
            GROUP BY ContractId
        ) as community_social ON c.Id = community_social.ContractId
        LEFT JOIN (
                SELECT 
                    ContractId,
                    COUNT(*) AS eligible_count
                FROM mi.ActivityPayment
                WHERE EligibleForPayment = 1  
                  AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
                  AND ActivityType IN ('ISW Support')
                GROUP BY ContractId
            ) as isws ON c.Id = isws.ContractId
        LEFT JOIN (
                SELECT 
                    ContractId,
                    COUNT(*) AS eligible_count
                FROM mi.EmploymentPayment
                WHERE EligibleForPayment = 1  
                  AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
                GROUP BY ContractId
            ) as employment ON c.Id = employment.ContractId
        LEFT JOIN (
                    SELECT 
                        ContractId,
                        COUNT(*) AS eligible_count
                    FROM mi.EducationPayment
                    WHERE EligibleForPayment = 1  
                      AND PaymentPeriod BETWEEN @StartDate AND @EndDate  
                    GROUP BY ContractId
                ) as education ON c.Id = education.ContractId
    )
    SELECT 
        contract_id,
        description,
        custody_enrolments,
        community_enrolments,
        wing_inductions,
        hub_inductions,
        prerelease_support,
        ttg,
        support_work,
        human_citizenship,
        community_and_social,
        isws,
        employment,
        education
    FROM payment_summary
);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS mi.GetCumulativeTotals;");
        }
    }
}
