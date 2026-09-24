


IF NOT EXISTS (SELECT TOP (1) LabelId FROM [Configuration].[LabelContract])
BEGIN

  INSERT INTO [Configuration].[LabelContract] ( [ContractId], [LabelId] )
  SELECT [Contract].[Id], [Label].[Id] FROM [Configuration].[Contract], [Configuration].[Label]
  WHERE Label.Id <> '1f611df9-1fb2-4059-b1ff-dcb4db6e761b' -- NO ISC generic
 
END