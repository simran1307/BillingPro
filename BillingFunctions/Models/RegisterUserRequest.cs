namespace BillingFunctions.Models;

public class RegisterUserRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}