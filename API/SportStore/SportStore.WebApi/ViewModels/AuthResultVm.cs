namespace SportStore.WebApi.ViewModels
{
    public class AuthResultVm
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}