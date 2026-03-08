-- פרצדורה לאישור/דחייה של בקשה ע"י פקיד

CREATE PROCEDURE ApproveOrRejectRefundRequest
(
    @RequestId INT,
    @IsApproved BIT,
    @ApprovedAmount DECIMAL(12,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;



    BEGIN TRY
        BEGIN TRANSACTION;

        -- שלב 1: שליפת נתוני הבקשה והסטטוס
        
        DECLARE @StatusCalculatedId INT;
        DECLARE @StatusApprovedId INT;
        DECLARE @StatusRejectedId INT;
        DECLARE @CurrentStatusId INT;
        DECLARE @CalculatedAmount DECIMAL(12,2);

        SELECT @StatusCalculatedId = Id FROM RequestStatuses WHERE Name = 'Calculated';
        SELECT @StatusApprovedId = Id FROM RequestStatuses WHERE Name = 'Approved';
        SELECT @StatusRejectedId = Id FROM RequestStatuses WHERE Name = 'Rejected';

        SELECT 
            @CurrentStatusId = StatusId,
            @CalculatedAmount = CalculatedAmount
        FROM RefundRequests
        WHERE Id = @RequestId;
 





        DECLARE @StatusPendingId INT;
        SELECT @StatusPendingId = Id FROM RequestStatuses WHERE Name = 'Pending';

        -- שלב 2: טיפול בדחייה – מותר גם כשלא חושב (Pending או Calculated)
        IF @IsApproved = 0
        BEGIN
            IF @CurrentStatusId NOT IN (@StatusPendingId, @StatusCalculatedId)
            BEGIN
                RAISERROR('לא ניתן לדחות בקשה במצב זה',16,1);
                ROLLBACK;
                RETURN;
            END

            UPDATE RefundRequests
            SET
                StatusId = @StatusRejectedId,
                ApprovedAmount = 0
            WHERE Id = @RequestId;

            COMMIT;
            RETURN;
        END

        -- אישור – הבקשה חייבת להיות במצב Calculated
        IF @CurrentStatusId <> @StatusCalculatedId
        BEGIN
            RAISERROR('ניתן לאשר רק בקשה במצב Calculated',16,1);
            ROLLBACK;
            RETURN;
        END


        -- שלב 3: בדיקות אישור
        IF @ApprovedAmount IS NULL OR @ApprovedAmount <= 0
        BEGIN
            RAISERROR('יש להזין סכום מאושר תקין',16,1);
            ROLLBACK;
            RETURN;
        END

        IF @ApprovedAmount > @CalculatedAmount
        BEGIN
            RAISERROR('לא ניתן לאשר סכום גבוה מהסכום המחושב',16,1);
            ROLLBACK;
            RETURN;
        END

        -- שלב 4: שליפת התקציב החודשי
        DECLARE @Year INT = YEAR(GETDATE());
        DECLARE @Month INT = MONTH(GETDATE());
        DECLARE @TotalBudget DECIMAL(14,2);
        DECLARE @UsedBudget DECIMAL(14,2);

        SELECT 
            @TotalBudget = TotalBudget,
            @UsedBudget = UsedBudget
        FROM Budgets WITH (UPDLOCK, ROWLOCK)
        WHERE [Year] = @Year AND [Month] = @Month;

        IF @TotalBudget IS NULL
        BEGIN
            RAISERROR('לא קיים תקציב לחודש זה',16,1);
            ROLLBACK;
            RETURN;
        END

        DECLARE @AvailableBudget DECIMAL(14,2);
        SET @AvailableBudget = @TotalBudget - @UsedBudget;

        IF @ApprovedAmount > @AvailableBudget
        BEGIN
            RAISERROR('אין מספיק תקציב פנוי לאישור הבקשה',16,1);
            ROLLBACK;
            RETURN;
        END

        
        -- שלב 5: עדכון התקציב
        
        UPDATE Budgets
        SET UsedBudget = UsedBudget + @ApprovedAmount
        WHERE [Year] = @Year AND [Month] = @Month;

        
        -- שלב 6: עדכון הבקשה
        UPDATE RefundRequests
        SET
            StatusId = @StatusApprovedId,
            ApprovedAmount = @ApprovedAmount
        WHERE Id = @RequestId;

        COMMIT;

    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END
