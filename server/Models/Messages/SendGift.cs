namespace server.Models.Messages
{
    public class SendGift : PlayerMessage
    {
        public SendGift() : base("SendGift") { }
        public string DeviceId { get; set; }
        public string PlayerId { get; set; }
        public string FriendId { get; set; }
        public ResourceType ResourceType { get; set; }
        public int ResourceValue { get; set; }
    }
}
