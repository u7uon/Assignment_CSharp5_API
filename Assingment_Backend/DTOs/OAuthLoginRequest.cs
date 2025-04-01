using static Assignment_Backend.Controllers.AuthController;

namespace Assingment_Backend.DTOs
{
    public class OAuthLoginRequest
    {
        public string Provider { get; set; }

        public string AuthorizationCode { get; set; }
    }
}
