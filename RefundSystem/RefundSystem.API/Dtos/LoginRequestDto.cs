namespace RefundSystem.API.Dtos;

//גוף בקשה להתחברות – תעודת זהות (POST login).
public sealed class LoginRequestDto
{
    //תעודת זהות של המשתמש.
    public string IdentityNumber { get; set; } = string.Empty;
}

