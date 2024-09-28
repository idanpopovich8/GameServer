using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Options;
using server.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using server;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; 
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; 
    options.JsonSerializerOptions.WriteIndented = true; // For pretty-printing
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPlayerService,PlayerService>();
builder.Services.AddSingleton<WebSocketServer>();
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(200);
});
builder.WebHost.UseUrls( "https://localhost:5000");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseWebSockets();
app.MapControllers(); // Map the HTTP API routes

// Configure WebSocket options
var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2), // Ping interval
    ReceiveBufferSize = 4 * 1024 // Buffer size for WebSocket messages
};
app.UseWebSockets(webSocketOptions);


app.Map("/ws", async (context) =>
{    if (context.WebSockets.IsWebSocketRequest)
    {
        var wsServer = context.RequestServices.GetRequiredService<WebSocketServer>();
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await wsServer.HandleWebSocketConnection(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400; // Bad Request if it's not a WebSocket request
    }
});

app.Run();

// Method to handle WebSocket communication

