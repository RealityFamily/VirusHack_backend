using System.Runtime.Serialization;

namespace VirusHack.WebApp.Requests
{
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
