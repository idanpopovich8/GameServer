namespace server.Exceptions
{
    public class PlayerAlreadyLoggedIn : Exception
    {
        public PlayerAlreadyLoggedIn (string DeviceId) : base (@$"Device {DeviceId} Already logged in") { }

    }
}
