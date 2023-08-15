using Newtonsoft.Json;

namespace Project.Model
{
    public class Deal
    {
        [JsonProperty(PropertyName = "storeID")]
        public string StoreId { get; set; }

        [JsonProperty(PropertyName = "dealID")]
        public string DealId { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float SalePrice { get; set; }

        [JsonProperty(PropertyName = "retailPrice")]
        public float NormalPrice { get; set; }

        [JsonProperty(PropertyName = "savings")]
        public float SavingPercentage { get; set; }
    }
}