namespace server.Models.Messages
{
    public class Login : PlayerMessage
    {
        public Login() : base("Login") { }

        public string DeviceId { get; set; }
    }
}
