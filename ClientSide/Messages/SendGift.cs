namespace Messages
{
    public class SendGift : PlayerMessage
    {

        public SendGift(string DeviceId, string PlayerId, string FriendId, ResourceType ResourceType, int ResourceValue) : base("SendGift")
        {
            this.DeviceId = DeviceId;
            this.PlayerId  = PlayerId;
            this.FriendId = FriendId;
            this.ResourceType = ResourceType;
            this.ResourceValue = ResourceValue;
        }
        public string DeviceId { get; set; }
        public string PlayerId { get; set; } 
        public string FriendId { get; set; }

        public ResourceType ResourceType { get; set; }
        public int ResourceValue { get; set; }
    }
}
