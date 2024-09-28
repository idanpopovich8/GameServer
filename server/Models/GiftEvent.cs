using server.Models.Messages;

namespace server.Models
{
    public class GiftEvent : ServerMessage
    {
        public GiftEvent() : base("GiftEvent") { }
        public ResourceType ResourceType { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int ResourceValue { get; set; }
    }
}
