namespace Educational_Platform.Application.Models.CommandModels
{
    public class AuthenticationRequestModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
