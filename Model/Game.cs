using Newtonsoft.Json;
using System.Collections.Generic;

namespace Project.Model
{
    public class Game
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "thumb")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "deals")]
        public List<Deal> Deals { get; set; }
    }
}