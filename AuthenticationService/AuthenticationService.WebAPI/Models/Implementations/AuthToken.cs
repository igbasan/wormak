namespace AuthenticationService.WebAPI.Models.Implementations
{
    public class AuthToken
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
