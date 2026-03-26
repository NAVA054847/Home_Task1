using System;
using System.Collections.Generic;

namespace RefundSystem.Core.Entities;

public partial class RefundRequest
{
    public int Id { get; set; }

    public int CitizenId { get; set; }

    public int TaxYear { get; set; }

    public int StatusId { get; set; }

    public decimal? CalculatedAmount { get; set; }

    public decimal? ApprovedAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CalculatedAt { get; set; }
//לכל בקשת החזר יש אזרח אחד.
    public virtual Citizen Citizen { get; set; } = null!;
//לכל בקשת החזר יש סטטוס אחד.
    public virtual RequestStatus Status { get; set; } = null!;
}
