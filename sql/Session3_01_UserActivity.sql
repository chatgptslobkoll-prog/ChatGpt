/*
Дополнительная таблица для сессии 1/3: трекинг входов/выходов пользователей.
Структура основной БД не изменяется; добавляется новая таблица + индексы.
*/

USE [Session3_01];
GO

IF OBJECT_ID(N'dbo.UserActivityLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserActivityLogs
    (
        ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserID INT NOT NULL,
        LoginAt DATETIME2(0) NOT NULL,
        LogoutAt DATETIME2(0) NULL,
        CrashReason NVARCHAR(500) NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_UserActivityLogs_CreatedAt DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_UserActivityLogs_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(ID)
    );

    CREATE INDEX IX_UserActivityLogs_UserID_LoginAt ON dbo.UserActivityLogs(UserID, LoginAt DESC);
END
GO
