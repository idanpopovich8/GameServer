namespace Messages
{
    public class Logout : PlayerMessage
    {

        public Logout(string DeviceId) : base("Logout")
        {
            this.DeviceId = DeviceId;
        }

        public string DeviceId { get; set; }
    }
}
