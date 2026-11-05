UPDATE [Configuration].[Label]
SET [Variant] = 0, [Colour] = 4
WHERE [Id] = 'c4962b22-8897-4771-b80f-334eb98b6920'
  AND ([Variant] <> 0 OR [Colour] <> 4);