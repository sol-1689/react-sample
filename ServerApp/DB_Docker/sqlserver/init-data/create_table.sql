

-- 新規作成するDBにテーブルを追加
USE owmlocal


GO
CREATE TABLE M_Division(
	DivisionNo int NOT NULL,
	DivisionName nvarchar(100) NOT NULL,
	Remarks nvarchar(100) NULL,
	OrderNo int NOT NULL,
	CreateDateTime datetime2 NOT NULL,
	UpdateDateTime datetime2 NOT NULL,
	DeleteFlag int NOT NULL,
	
	CONSTRAINT PK_M_Division PRIMARY KEY CLUSTERED
	(
		DivisionNo
	)
)

GO
CREATE SEQUENCE [dbo].[Seq_M_Division] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE 1
 MAXVALUE 2147483647
 CYCLE 
 CACHE 
GO
ALTER TABLE M_Division ADD  CONSTRAINT [DF_M_Division_DivisionNo]  DEFAULT (NEXT VALUE FOR Seq_M_Division) FOR DivisionNo


GO
CREATE TABLE M_General(
	GeneralNo int NOT NULL,
	GeneralName nvarchar(100) NULL,
	Remarks nvarchar(100) NULL,
	Description nvarchar(100) NULL,
	CategoryId varchar(100) NOT NULL,
	CategoryName nvarchar(100) NOT NULL,
	OrderNo int NOT NULL,
	CreateDateTime datetime2 NOT NULL,
	UpdateDateTime datetime2 NOT NULL,
	DeleteFlag int NOT NULL,

	CONSTRAINT PK_M_General PRIMARY KEY CLUSTERED
	(
		GeneralNo
	)
)

GO
CREATE SEQUENCE [dbo].[Seq_M_General] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE 1
 MAXVALUE 2147483647
 CYCLE 
 CACHE 
GO
ALTER TABLE M_General ADD  CONSTRAINT [DF_M_General_GeneralNo]  DEFAULT (NEXT VALUE FOR Seq_M_General) FOR GeneralNo



GO
CREATE TABLE M_User(
	UserNo int NOT NULL,
	UserName nvarchar(100) NOT NULL,
	LoginId varchar(100) NOT NULL,
	Password varbinary(MAX) NOT NULL,
	Role int NULL,
	CreateBy int NOT NULL,
	CreateDateTime datetime2 NOT NULL,
	UpdateBy int NOT NULL,
	UpdateDateTime datetime2 NOT NULL,
	DeleteFlag int NOT NULL,
	
	CONSTRAINT PK_M_User PRIMARY KEY CLUSTERED	
	(
		UserNo
	)
)

GO
CREATE SEQUENCE [dbo].[Seq_M_User] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE 1
 MAXVALUE 2147483647
 CYCLE 
 CACHE 
GO
ALTER TABLE M_User ADD  CONSTRAINT [DF_M_User_UserNo]  DEFAULT (NEXT VALUE FOR Seq_M_User) FOR UserNo
GO


GO
CREATE TABLE M_UserDivision(
	UserNo int NOT NULL,
	DivisionNo  int NOT NULL,
	
	CONSTRAINT PK_M_UserDivision PRIMARY KEY CLUSTERED	
	(
		UserNo, DivisionNo
	)
)

GO
CREATE TABLE M_BusinessCode(
	BusinessCodeNo int NOT NULL,
	AccountingCodeNo int NOT NULL,
	DebitBusinessCode varchar(10) NOT NULL,
	DebitBusinessName nvarchar(100) NOT NULL,
	DebitAccountingItemCode varchar(10) NOT NULL,
	DebitAccountingAssistItemCode varchar(10) NOT NULL,
	DebitTaxCode varchar(10) NOT NULL,
	CreditBusinessCode varchar(10) NOT NULL,
	CreditAccountingItemCode varchar(10) NOT NULL,
	CreditAccountingAssistItemCode varchar(10) NOT NULL,
	CreditTaxCode varchar(10) NOT NULL,
	PaymentFlag int NOT NULL,
	CreateBy int NOT NULL,
	CreateDateTime datetime2 NOT NULL,
	UpdateBy int NOT NULL,
	UpdateDateTime datetime2 NOT NULL,
	DeleteFlag int NOT NULL,

	CONSTRAINT PK_M_BusinessCode PRIMARY KEY CLUSTERED
	(
		BusinessCodeNo
	)
)

GO
CREATE SEQUENCE [dbo].[Seq_M_BusinessCode] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE 1
 MAXVALUE 2147483647
 CYCLE 
 CACHE 
GO
ALTER TABLE M_BusinessCode ADD  CONSTRAINT [DF_M_BusinessCode_BusinessCodeNo]  DEFAULT (NEXT VALUE FOR Seq_M_BusinessCode) FOR BusinessCodeNo
