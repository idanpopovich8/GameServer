using server.Models;

namespace server
{
    public class Utils
    {
        public static  string FromResourceType(ResourceType type)
        {
            if (type == ResourceType.Rolls) return "Rolls";
            return "Coins";
        }
    }
}
