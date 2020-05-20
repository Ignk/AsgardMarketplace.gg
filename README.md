# AsgardMarketplace.gg

**Connect solution to local db with table (update appsetting.json ConnectionStrings/DefaultConnection) :**

CREATE TABLE [dbo].[Orders] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [Item]      VARCHAR (MAX) NOT NULL,
    [Customer]  VARCHAR (50)  NOT NULL,
    [Ordered]   DATETIME      NOT NULL,
    [Paid]      BIT           NOT NULL,
    [Delivered] BIT           NOT NULL
);

**Run solution on VS 2019**