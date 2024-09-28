namespace Messages
{
    public class Login : PlayerMessage
    {
        public Login(string DeviceId) : base("Login")
        {
            this.DeviceId = DeviceId;
        }

        public string DeviceId { get; set; }
        
    }
}
