


using Messages;
using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;

public class ClientTest
{


    private static string? PlayerId = null;

    private static string DeviceId = Guid.NewGuid().ToString(); // device id

    private static async Task ReceiveMessagesAsync(ClientWebSocket client,CancellationToken token)
    {
        var buffer = new byte[1024];
        while(client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), token); 
            if(result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Server initiated close. closing connection..");
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", token);
                break;
            }
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var serverMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerMessage>(message);
            if(serverMessage?.Type == "Login")
            {

                PlayerId = serverMessage.Data;
            }
            
            Console.WriteLine($"{serverMessage?.Message}");
        }
    }

    private static async Task SendMessagesAsync(ClientWebSocket client, CancellationToken token)
    {

        while(client.State == WebSocketState.Open)
        {
            Console.WriteLine("Enter message to send\n1) Login\n2) Logout\n3) Update resources\n4) Send Gift\n5) Exit");
            var option = int.Parse(Console.ReadLine());

            if(option == 5)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", token);
                break;
            }
            string? encoded = null;
            switch(option)
            {
                case 1:
                    encoded = Newtonsoft.Json.JsonConvert.SerializeObject(new Login(DeviceId: DeviceId));
                    break;
                case 2:
                    if (PlayerId == null)
                    {
                        Console.WriteLine("Login first");
                        continue;
                    }
                    encoded = Newtonsoft.Json.JsonConvert.SerializeObject(new Logout(DeviceId: DeviceId));
                    break;
                case 3:
                    if(PlayerId == null)
                    {
                        Console.WriteLine("Login first");
                        continue;
                    }
                    Console.WriteLine("Enter Resource type to update\n1) Coins\n2) Rolls");
                    int opt = int.Parse(Console.ReadLine());
                    ResourceType type = opt == 1 ? ResourceType.Coins : ResourceType.Rolls;
                    Console.WriteLine("Enter amount to add:");
                    int amount = int.Parse(Console.ReadLine());
                    encoded = Newtonsoft.Json.JsonConvert.SerializeObject(new UpdateResources(PlayerId: PlayerId, type, amount));
                    break;
                case 4:
                    if (PlayerId == null)
                    {
                        Console.WriteLine("Login first");
                        continue;
                    }
                    Console.WriteLine("Enter Friend id");
                    string friendId = Console.ReadLine();
                    Console.WriteLine("Enter Resource type to send\n 1) Coins\n2) Rolls");
                    int _opt = int.Parse(Console.ReadLine());
                    ResourceType _type = _opt == 1 ? ResourceType.Coins : ResourceType.Rolls;
                    Console.WriteLine("Enter amount to add:");
                    int _amount = int.Parse(Console.ReadLine());
                    encoded = Newtonsoft.Json.JsonConvert.SerializeObject(new SendGift(DeviceId: DeviceId, PlayerId: PlayerId, FriendId:friendId, _type,_amount));
                    break;
            }
            if(encoded != null)
            {
                var bytesToSend = Encoding.UTF8.GetBytes(encoded);
                var bytesSegment = new ArraySegment<byte>(bytesToSend);
                await client.SendAsync(bytesSegment, WebSocketMessageType.Text, endOfMessage: true, token);
                Console.WriteLine($@"Sent: {encoded}");
            }
           else
            {
                Console.WriteLine($@"Error sening message");
            }
        }
    }

    public static async Task Main(string[] args)
    {

        var uri = new Uri("wss://localhost:5000/ws");

        using var client = new ClientWebSocket();
        var cts = new CancellationTokenSource();
        try
        {
            await client.ConnectAsync(uri, cts.Token);

            var receieveTask = ReceiveMessagesAsync(client, cts.Token);
            var sendTask = SendMessagesAsync(client, cts.Token);

            await Task.WhenAny(receieveTask, sendTask);
            cts.Cancel();
            await Task.WhenAll(receieveTask, sendTask); 
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
 }