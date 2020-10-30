IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].Groups') AND name = 'g_catagory')
BEGIN
ALTER TABLE [dbo].[Groups] ADD g_catagory varchar(10)
END

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].bank_payments') AND name = 'bp_invoice')
BEGIN
ALTER TABLE [dbo].[bank_payments] ADD bp_invoice varchar(20)
END


IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].bank_receipts') AND name = 'br_invoice')
BEGIN
ALTER TABLE [dbo].[bank_receipts] ADD br_invoice varchar(20)
END