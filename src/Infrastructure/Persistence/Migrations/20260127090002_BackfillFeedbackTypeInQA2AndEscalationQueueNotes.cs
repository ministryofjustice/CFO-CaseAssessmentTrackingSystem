using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cfo.Cats.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BackfillFeedbackTypeInQA2AndEscalationQueueNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Enrolment QA2
                UPDATE qn
                SET qn.FeedbackType =
                    CASE
                        WHEN qn.[Message] LIKE N'Advisory%' THEN 0
                        WHEN qn.[Message] LIKE N'Accepted by Exception%' THEN 1
                        ELSE NULL
                    END
                FROM [Enrolment].[Qa2QueueNote] AS qn
                INNER JOIN [Enrolment].[Qa2Queue] AS q
                    ON q.[Id] = qn.[EnrolmentQa2QueueEntryId]
                WHERE
                    (qn.[Message] LIKE N'Advisory%' OR qn.[Message] LIKE N'Accepted by Exception%')
                    AND q.[IsAccepted] = CAST(1 AS bit)
                    AND q.[IsCompleted] = CAST(1 AS bit);

                -- Enrolment Escalation
                UPDATE en
                SET en.FeedbackType =
                    CASE
                        WHEN en.[Message] LIKE N'Advisory%' THEN 0
                        WHEN en.[Message] LIKE N'Accepted by Exception%' THEN 1
                        ELSE NULL
                    END
                FROM [Enrolment].[EscalationNote] AS en
                INNER JOIN [Enrolment].[EscalationQueue] AS eq
                    ON eq.[Id] = en.[EnrolmentEscalationQueueEntryId]
                WHERE
                    (en.[Message] LIKE N'Advisory%' OR en.[Message] LIKE N'Accepted by Exception%')
                    AND eq.[IsAccepted] = CAST(1 AS bit)
                    AND eq.[IsCompleted] = CAST(1 AS bit);

                -- Activities QA2
                UPDATE qn
                SET qn.FeedbackType =
                    CASE
                        WHEN qn.[Message] LIKE N'Advisory%' THEN 0
                        WHEN qn.[Message] LIKE N'Accepted by Exception%' THEN 1
                        ELSE NULL
                    END
                FROM [Activities].[Qa2QueueNote] AS qn
                INNER JOIN [Activities].[ActivityQa2Queue] AS aq
                    ON aq.[Id] = qn.[ActivityQa2QueueEntryId]
                WHERE
                    (qn.[Message] LIKE N'Advisory%' OR qn.[Message] LIKE N'Accepted by Exception%')
                    AND aq.[IsAccepted] = CAST(1 AS bit)
                    AND aq.[IsCompleted] = CAST(1 AS bit);

                -- Activities Escalation
                UPDATE en
                SET en.FeedbackType =
                    CASE
                        WHEN en.[Message] LIKE N'Advisory%' THEN 0
                        WHEN en.[Message] LIKE N'Accepted by Exception%' THEN 1
                        ELSE NULL
                    END
                FROM [Activities].[EscalationNote] AS en
                INNER JOIN [Activities].[ActivityEscalationQueue] AS eq
                    ON eq.[Id] = en.[ActivityEscalationQueueEntryId]
                WHERE
                    (en.[Message] LIKE N'Advisory%' OR en.[Message] LIKE N'Accepted by Exception%')
                    AND eq.[IsAccepted] = CAST(1 AS bit)
                    AND eq.[IsCompleted] = CAST(1 AS bit);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Revert Enrolment QA2
                UPDATE [Enrolment].[Qa2QueueNote]
                SET FeedbackType = NULL
                WHERE FeedbackType IN (0, 1);

                -- Revert Enrolment Escalation
                UPDATE [Enrolment].[EscalationNote]
                SET FeedbackType = NULL
                WHERE FeedbackType IN (0, 1);

                -- Revert Activities QA2
                UPDATE [Activities].[Qa2QueueNote]
                SET FeedbackType = NULL
                WHERE FeedbackType IN (0, 1);

                -- Revert Activities Escalation
                UPDATE [Activities].[EscalationNote]
                SET FeedbackType = NULL
                WHERE FeedbackType IN (0, 1);
            ");
        }
    }
}
