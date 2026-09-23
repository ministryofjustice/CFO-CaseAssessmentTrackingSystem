


IF NOT EXISTS (SELECT TOP (1) LabelId FROM [Configuration].[LabelContract])
BEGIN

  INSERT INTO [Configuration].[LabelContract] ( [ContractId], [LabelId] )
  SELECT [Contract].[Id], [Label].[Id] FROM [Configuration].[Contract], [Configuration].[Label]
 
END