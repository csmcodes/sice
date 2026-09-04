namespace WebAPI.Models
{
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public string token { get; set; }
        public int expireMinutes { get; set; }
    }
}
