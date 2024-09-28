namespace server.Models.Messages
{
    public class Logout : PlayerMessage
    {
        public Logout() : base("Logout") { }
        public string DeviceId { get; set; }
    }
}
