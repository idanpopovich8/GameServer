namespace server.Exceptions
{
    public class PlayerNotLoggedIn : Exception
    {
        public PlayerNotLoggedIn(string DeviceId)  : base($@"Player with Device id {DeviceId} Not logged in"){ }
    }
}
