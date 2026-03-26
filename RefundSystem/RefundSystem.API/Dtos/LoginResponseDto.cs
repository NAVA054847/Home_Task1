namespace RefundSystem.API.Dtos;

//תגובת התחברות – תפקיד, מזהה ושם מלא (מה שה-API מחזיר ללקוח).
public sealed class LoginResponseDto
{       
    //תפקיד: Clerk או Citizen.
    public string Role { get; set; } = string.Empty;

    //מזהה הפקיד או האזרח.
    public int Id { get; set; }

    //שם מלא.
    public string FullName { get; set; } = string.Empty;
}

