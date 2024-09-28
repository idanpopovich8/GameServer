namespace Messages
{
    public class UpdateResources: PlayerMessage
    {

        public UpdateResources(string PlayerId, ResourceType ResourceType, int ResourceValue) : base("UpdateResources")
        {
            this.PlayerId = PlayerId;
            this.ResourceType = ResourceType;
            this.ResourceValue = ResourceValue;
        }
        public string PlayerId { get; set; }
        public ResourceType ResourceType { get; set; }
        public int ResourceValue { get; set; }
    }
}
