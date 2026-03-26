namespace RefundSystem.API.Dtos;

//גוף בקשה לאישור או דחיית בקשת החזר (POST approve).
public sealed class ApproveRefundRequestDto
{
    //מזהה הבקשה.
    public int RequestId { get; set; }

    //true = אישור, false = דחייה.
    public bool IsApproved { get; set; }

    //סכום מאושר – רלוונטי רק באישור; בדחייה null.
    public decimal? ApprovedAmount { get; set; }
}

