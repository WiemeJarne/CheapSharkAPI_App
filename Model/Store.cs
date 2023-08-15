using Newtonsoft.Json;

namespace Project.Model
{
    public class Store
    {
        [JsonProperty(PropertyName = "storeID")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "storeName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "banner")]
        public string BannerUrl { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string IconUrl { get; set; }
    }
}
