namespace Messages
{
    public class ServerMessage
    {
        public ServerMessage(string Type) { this.Type = Type;  }

        public string Type { get; set; }
        public string Data { get; set; }

        public string Message { get; set; } 
        
    }
}
