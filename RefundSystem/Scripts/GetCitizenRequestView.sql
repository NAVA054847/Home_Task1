-- משתמשת בפרוצדורה הקיימת GetCitizenRequestsHistory. כל הלוגיקה ב-SQL.
-- מחזירה: Result set 1 = הבקשה האחרונה (שורה אחת או ריק), Result set 2 = היסטוריה ללא הבקשה האחרונה.
CREATE OR ALTER PROCEDURE GetCitizenRequestView
    @IdentityNumber NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CitizenId INT;
    SELECT @CitizenId = Id FROM Citizens WHERE IdentityNumber = @IdentityNumber;

    CREATE TABLE #History (
        RequestId INT,
        TaxYear INT,
        CalculatedAmount DECIMAL(18,2),
        ApprovedAmount DECIMAL(18,2),
        Status NVARCHAR(50),
        CreatedAt DATETIME
    );

    IF @CitizenId IS NOT NULL
        INSERT INTO #History
        EXEC GetCitizenRequestsHistory @CitizenId;

    -- Result set 1: הבקשה האחרונה (שורה אחת או ריק)
    SELECT TOP 1 RequestId, TaxYear, CalculatedAmount, ApprovedAmount, Status, CreatedAt
    FROM #History
    ORDER BY CreatedAt DESC;

    -- Result set 2: היסטוריה ללא הבקשה האחרונה
    SELECT H.RequestId, H.TaxYear, H.CalculatedAmount, H.ApprovedAmount, H.Status, H.CreatedAt
    FROM #History H
    WHERE H.RequestId <> (SELECT TOP 1 RequestId FROM #History ORDER BY CreatedAt DESC)
       OR (SELECT TOP 1 RequestId FROM #History ORDER BY CreatedAt DESC) IS NULL
    ORDER BY H.CreatedAt DESC;

    DROP TABLE #History;
END;
GO
