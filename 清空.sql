delete from [dbo].[BizProcessesAuditDetails]
delete from [dbo].[BizProcessesAudit]

delete from [dbo].[OrderPayResultNotifyLog]
delete from [dbo].[OrderToCarClaim]
delete from [dbo].[OrderToCarInsure]
delete from [dbo].[OrderToCarInsureOfferCompany]
delete from [dbo].[OrderToCarInsureOfferKind]
delete from [dbo].[OrderToDepositRent]
delete from [dbo].[Order]

delete [dbo].[BankCard]

delete from [dbo].[Fund] where UserId not in('1','2','3','4')

update [dbo].[Fund] set [Balance]=0,[Arrearage]=0


delete from [dbo].[SalesmanApplyPosRecord]
delete from [dbo].[Transactions]
delete from [dbo].[WithdrawCutOff]
delete from [dbo].[WithdrawCutOffDetails]
delete from [dbo].[Withdraw]
delete from [dbo].[MerchantPosMachine]
delete from [dbo].[MerchantEstimateCompany]
delete from [dbo].[Merchant]


update PosMachine set [IsUse]=0


