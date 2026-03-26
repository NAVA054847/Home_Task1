using System;
using System.Collections.Generic;

namespace RefundSystem.Core.Entities;

public partial class Citizen
{
    public int Id { get; set; }

    public string IdentityNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

//לכל אזרח יש מספר הכנסות חודשיות.
    public virtual ICollection<MonthlyIncome> MonthlyIncomes { get; set; } = new List<MonthlyIncome>();
//לכל אזרח יש מספר בקשות החזר.
    public virtual ICollection<RefundRequest> RefundRequests { get; set; } = new List<RefundRequest>();
}
