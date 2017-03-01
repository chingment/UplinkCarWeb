select  * from [dbo].[Fund]

delete from [dbo].[Transactions]
delete from  [dbo].[Withdraw]
delete from [dbo].[WithdrawCutOff]
delete from  [dbo].[WithdrawCutOffDetails]
update Fund set [Balance]=0,[Arrearage]=0 

update [dbo].[Fund] set [Balance]=10000 where userid='1001'


select * from [dbo].[Fund]
select * from [dbo].[Transactions]

select * from [dbo].[Withdraw]
select * from [dbo].[WithdrawCutOff]
select * from [dbo].[WithdrawCutOffDetails]