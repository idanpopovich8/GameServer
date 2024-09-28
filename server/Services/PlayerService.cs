using server.Exceptions;
using server.Models;
using System.Diagnostics;
using System.Net.WebSockets;

namespace server.Services
{
    public class PlayerService : IPlayerService
    {

        public Dictionary<string, Player> Players = new Dictionary<string, Player>();
        public Dictionary<string, string> PlayerByDevice = new Dictionary<string, string>();

        public Dictionary<string, Player> ConnectedPlayers = new Dictionary<string, Player>();

        public PlayerLoginResult Login(string DeviceId)
        {
           if(ConnectedPlayers.Values.Any(p => p.DeviceId == DeviceId))
            {
                throw new PlayerAlreadyLoggedIn(DeviceId);
            }
            Debug.WriteLine(DeviceId);
            Player player;
            if (Players.TryGetValue(DeviceId, out player))
            {
                // connect the player
                player.IsLoggedIn = true;
                ConnectedPlayers.Add(DeviceId, player);
                PlayerByDevice.Add(player.PlayerId, DeviceId);
                return new PlayerLoginResult
                {
                    Player = player,
                    WasLoggedIn = true
                };
            }
            else
            {
                player = new Player
                {
                    DeviceId = DeviceId,
                    PlayerId = Guid.NewGuid().ToString(),
                    IsLoggedIn = true
                };

                ConnectedPlayers.Add(DeviceId, player);
                Players.Add(player.DeviceId, player);
                PlayerByDevice.Add(player.PlayerId, DeviceId);
                return new PlayerLoginResult
                {
                    Player = player,
                    WasLoggedIn = false
                }; ;
            }
        }


        public Player Logout(string DeviceId)
        {
            if (ConnectedPlayers.ContainsKey(DeviceId))
            {
                var connectedPlayer = ConnectedPlayers[DeviceId];
                connectedPlayer.IsLoggedIn = false;
                ConnectedPlayers.Remove(DeviceId);
                PlayerByDevice.Remove(connectedPlayer.PlayerId);
                return connectedPlayer;
            }
            throw new PlayerNotLoggedIn(DeviceId);
        }
        public GiftEvent SendGift(string PlayerId, string FriendPlayerId, ResourceType ResourceType, int ResourceValue)
        {
            if(!PlayerByDevice.ContainsKey(PlayerId))
            {
                throw new PlayerNotFound(PlayerId);
            }
            if(!PlayerByDevice.ContainsKey(FriendPlayerId))
            {
                throw new PlayerNotFound(FriendPlayerId);
            }
            var FriendDeviceId = PlayerByDevice[FriendPlayerId];

            var Friend = Players[FriendDeviceId];
           
            Friend.UpdateResources(ResourceType, ResourceValue);
            return new GiftEvent
            {
                Message = $@"Received {ResourceValue} of {Utils.FromResourceType(ResourceType)} from {PlayerId}",
                Sender = PlayerId,
                Receiver = FriendPlayerId,
                ResourceType = ResourceType,
                ResourceValue = ResourceValue
            };
        }

        public int UpdateResources(string PlayerId, ResourceType ResourceType, int ResourceValue)
        {
           if(!PlayerByDevice.ContainsKey(PlayerId))
                throw new PlayerNotFound(PlayerId);
            var deviceId = PlayerByDevice[PlayerId];
            var player = Players[deviceId];
            return player.UpdateResources(ResourceType, ResourceValue);
        }
    }
}
