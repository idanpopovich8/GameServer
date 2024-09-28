namespace server.Models.Messages
{
    public class UpdateResources: PlayerMessage
    {

        public UpdateResources() : base("UpdateResources") { }

        public string PlayerId { get; set; }
        public ResourceType ResourceType { get; set; }
        public int ResourceValue { get; set; }
    }
}
