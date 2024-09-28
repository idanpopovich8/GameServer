using System.Net.WebSockets;

namespace server.Models
{
    public class Player
    {
        public Dictionary<ResourceType, int> Resources { get; set; } = new Dictionary<ResourceType, int>();
        public string PlayerId { get; set; }
        public string DeviceId { get; set; }
        public bool IsLoggedIn { get; set; }


        public int UpdateResources(ResourceType resourceType, int ResourceValue)
        {
            if (!Resources.ContainsKey(resourceType))
            {
                Resources[resourceType] = ResourceValue;
                return ResourceValue;
            }
            Resources[resourceType] += ResourceValue;
            return Resources[resourceType];
        }
    }
}
