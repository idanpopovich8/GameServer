namespace server.Exceptions
{
    public class PlayerNotFound : Exception
    {
        public PlayerNotFound(string PlayerId)  : base($@"Player with id {PlayerId} Not found"){ }
    }
}
