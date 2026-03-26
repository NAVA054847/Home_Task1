namespace RefundSystem.API.Dtos;

//גוף בקשה ליצירת בקשת החזר – מזהה אזרח ושנת מס (POST ליצירת בקשה).
public sealed class CreateRefundRequestDto
{
    //מזהה האזרח.
    public int CitizenId { get; set; }

    //שנת המס.
    public int TaxYear { get; set; }
}

