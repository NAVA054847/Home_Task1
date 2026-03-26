namespace RefundSystem.Core.ReadModels;

/// <summary>נתוני גלם מפרוצדורת פרטי בקשה – כולל כל הבקשות (כולל הנוכחית). הסינון ל־היסטוריה בלבד מתבצע ב־Service.</summary>
public sealed record ClerkRequestDetailsRaw(
    RefundRequestSummaryReadModel CurrentRequest,
    IReadOnlyList<MonthlyIncomeReadModel> Incomes,
    IReadOnlyList<RefundRequestSummaryReadModel> AllPastRequests,
    BudgetReadModel? CurrentMonthBudget);
