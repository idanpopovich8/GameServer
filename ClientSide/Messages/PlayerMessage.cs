namespace Messages
{
    public class PlayerMessage
    {

        public PlayerMessage(string Type)
        {
            this.Type = Type;
        }
        public string Type { get; set; }
    }
}
