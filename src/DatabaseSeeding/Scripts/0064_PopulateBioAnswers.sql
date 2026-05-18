IF NOT EXISTS (SELECT TOP(1) 1 FROM [Participant].[BioAnswer])
BEGIN

    -- Single-choice questions: each question object has { "Answer": "..." }
    -- A third OPENJSON level opens each question object; filtering on ans.[key] = 'Answer'
    -- naturally excludes the pathway-level $type row (non-JSON value produces no rows via CROSS APPLY)
    -- and there are no multi-choice questions for Bio.
    INSERT INTO [Participant].[BioAnswer] ([BioId], [QuestionCode], [Answer])
    SELECT
        a.[Id],
        q.[key],
        ans.[value]
    FROM [Participant].[Bio] a
    CROSS APPLY OPENJSON(a.[BioJson], '$.Pathways') pathways
    CROSS APPLY OPENJSON(pathways.[value]) q
    CROSS APPLY OPENJSON(q.[value]) ans
    WHERE q.[key] != '$type'
      AND ans.[key] = 'Answer'
      AND ans.[value] is not null;
END