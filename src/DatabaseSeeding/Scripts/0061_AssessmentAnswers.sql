IF NOT EXISTS (SELECT TOP(1) 1 FROM [Participant].[AssessmentAnswer])
BEGIN

    -- Single-choice questions: each question object has { "Answer": "..." }
    -- A third OPENJSON level opens each question object; filtering on ans.[key] = 'Answer'
    -- naturally excludes the pathway-level $type row (non-JSON value produces no rows via CROSS APPLY)
    -- and multi-choice questions (which have Answers, not Answer).
    INSERT INTO [Participant].[AssessmentAnswer] ([AssessmentId], [QuestionCode], [Answer])
    SELECT
        a.[Id],
        q.[key],
        ans.[value]
    FROM [Participant].[Assessment] a
    CROSS APPLY OPENJSON(a.[AssessmentJson], '$.Pathways') pathways
    CROSS APPLY OPENJSON(pathways.[value]) q
    CROSS APPLY OPENJSON(q.[value]) ans
    WHERE q.[key] != '$type'
      AND ans.[key] = 'Answer'
      AND ans.[value] is not null;

    -- Multiple-choice questions: each question object has { "Answers": { "$type": "...", "$values": [...] } }
    -- Newtonsoft wraps IEnumerable<string> as {"$type":"...","$values":[...]} with TypeNameHandling.Auto.
    -- ans opens the question object; v opens the Answers wrapper and steps directly into $values.
    -- "$values" must be quoted in the path because $ is reserved in SQL Server JSON paths.
    INSERT INTO [Participant].[AssessmentAnswer] ([AssessmentId], [QuestionCode], [Answer])
    SELECT
        a.[Id],
        q.[key],
        v.[value]
    FROM [Participant].[Assessment] a
    CROSS APPLY OPENJSON(a.[AssessmentJson], '$.Pathways') pathways
    CROSS APPLY OPENJSON(pathways.[value]) q
    CROSS APPLY OPENJSON(q.[value]) ans
    CROSS APPLY OPENJSON(ans.[value], '$."$values"') v
    WHERE q.[key] != '$type'
      AND ans.[key] = 'Answers'
      and v.[value] is not null;

END
