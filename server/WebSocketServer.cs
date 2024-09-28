using Serilog;
using server.Exceptions;
using server.Models;
using server.Models.Messages;
using server.Services;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace server
{
    public class WebSocketServer
    {

        public Dictionary<string, WebSocket> Connections = new Dictionary<string, WebSocket> ();
        public Dictionary<string, string> Players = new Dictionary<string, string>(); // DeviceId to player id
        public Dictionary<string, string> PlayerIdToDeviceId = new Dictionary<string, string>(); // Player id to device id

        public IPlayerService _playerService { get; set; }

        public WebSocketServer(IPlayerService playerService) {
            _playerService = playerService;
        }

       
        private PlayerMessage? DecodeMessage(string receivedMessage)
        {
            var message = JsonSerializer.Deserialize<PlayerMessage>(receivedMessage);

            if (message != null)
            {
                // check message type
                var type = message.Type;
                switch (type)
                {
                    case "Login":
                        return JsonSerializer.Deserialize<Login>(receivedMessage);
                    case "Logout":
                        return JsonSerializer.Deserialize<Logout>(receivedMessage);
                    case "SendGift":
                        return JsonSerializer.Deserialize<SendGift>(receivedMessage);
                    case "UpdateResources":
                        return JsonSerializer.Deserialize<UpdateResources>(receivedMessage);
                }
            }
            return null;
        }

        private async Task<ServerMessage> HandlePlayerMessage(PlayerMessage? message, WebSocket ws)
        {
            if (message == null) throw new InvalidMessageTypeException();
            if(message is Login)
            {
                var login = (Login)message;
                var playerLoginResult = _playerService.Login(login.DeviceId);
                Connections.Add(login.DeviceId, ws);
                Players.Add(login.DeviceId, playerLoginResult.Player.PlayerId);
                PlayerIdToDeviceId.Add(playerLoginResult.Player.PlayerId, login.DeviceId);
                return new ServerMessage("Login")
                {
                    Data = playerLoginResult.Player.PlayerId,
                    Message = playerLoginResult.WasLoggedIn ? $@"Welcome back {playerLoginResult.Player.PlayerId}" : $@"player with Id: {(playerLoginResult.Player.PlayerId)} logged in "
                };
            }
            else if(message is Logout)
            {
                var logout = (Logout)message;
                var player = _playerService.Logout(logout.DeviceId);
                Connections.Remove(logout.DeviceId);
                Players.Remove(logout.DeviceId);
                PlayerIdToDeviceId.Remove(player.PlayerId);
                return new ServerMessage("Logout")
                {
                    Data = player.PlayerId,
                    Message = "Logout successful"
                };
            }
            else if(message is SendGift)
            {
                var sendGift = (SendGift)message;
                var giftEvent = _playerService.SendGift(sendGift.PlayerId, sendGift.FriendId, sendGift.ResourceType, sendGift.ResourceValue);
                string? FriendDeviceId;

                if(PlayerIdToDeviceId.TryGetValue(sendGift.FriendId, out FriendDeviceId))
                {
                
                    byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(giftEvent));
                    await Connections[FriendDeviceId].SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, endOfMessage: true, new CancellationToken());
                }
                return new ServerMessage("SendGift")
                {
                    Data = JsonSerializer.Serialize(giftEvent),
                    Message = "Sent gift successfully"
                };
            }
            else if(message is UpdateResources)
            {
                var updateResources = (UpdateResources)message;
                var afterUpdate = _playerService.UpdateResources(updateResources.PlayerId, updateResources.ResourceType, updateResources.ResourceValue);
                return new ServerMessage("UpdateResources")
                {
                    Data = afterUpdate.ToString(),
                    Message = $@"Resource {Utils.FromResourceType(updateResources.ResourceType)} updated successfully to {afterUpdate}"
                };
            }
            return new ServerMessage("Error")
            {
                Data = "N/A",
                Message = "Message type invalid"
            };
        }


        private async Task<ServerMessage> ReceiveMessage(string message, WebSocket ws)
        {
            try
            {
                if (message != null)
                {
                    ServerMessage handled = await HandlePlayerMessage(DecodeMessage(message), ws);
                    if (handled == null)
                        throw new Exception("Corrupted message");
                    // handle message and user PlayerService accordingly
                    Console.WriteLine($"Received: {message}");
                    return handled;
                }
                else
                { // send error message
                    throw new Exception("Corrupted message");
                }
            }
            catch (Exception e)
            {
                return new ServerMessage("Error")
                {
                    Data = "N/A",
                    Message = e.Message
                };
            }
        }
          public async Task HandleWebSocketConnection(WebSocket webSocket)
            {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                string serverMessage = JsonSerializer.Serialize ( await ReceiveMessage(receivedMessage, webSocket ) );
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(serverMessage), 0, serverMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            // Close the WebSocket connection
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Console.WriteLine("WebSocket connection closed");
        }
    }
}
