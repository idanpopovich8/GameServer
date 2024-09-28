using server.Models;

namespace server.Services
{
    public interface IPlayerService
    {
        PlayerLoginResult Login(string DeviceId);
        Player Logout(string DeviceId);
        int UpdateResources(string PlayerId, ResourceType ResourceType, int ResourceValue);
        GiftEvent SendGift(string PlayerId, string FriendPlayerId, ResourceType ResourceType, int ResourceValue);
    }
}
