using Newtonsoft.Json;
using System.Collections.Generic;

namespace Occtoo.Formatter.Brink.Models
{
    public class Dimensions
    {
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class ProductModel
    {
        [JsonProperty("taxGroupId")]
        public string TaxGroupId { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayNames")]
        public Dictionary<string, string> DisplayNames { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("displayDescriptions")]
        public Dictionary<string, string> DisplayDescriptions { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("productParentId")]
        public string ProductParentId { get; set; }

        [JsonProperty("customAttributes")]
        public Dictionary<string, string> CustomAttributes { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, string> Tags { get; set; }

        [JsonProperty("dimensions")]
        public Dimensions Dimensions { get; set; }
    }
}
