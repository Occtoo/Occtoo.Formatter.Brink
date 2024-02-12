using Newtonsoft.Json;
using System.Collections.Generic;

namespace Occtoo.Formatter.Brink.Models
{
    // modify to how your api looks
    public class Medium
    {
        [JsonProperty("parentArticle")]
        public List<object> ParentArticle { get; set; }

        [JsonProperty("articleId")]
        public List<string> ArticleId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }
    }

    public class OcctooProductResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subCategory")]
        public string SubCategory { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("stock")]
        public string Stock { get; set; }

        [JsonProperty("descriptionSe")]
        public string DescriptionSe { get; set; }

        [JsonProperty("weight")]
        public int? Weight { get; set; }

        [JsonProperty("price")]
        public List<object> Price { get; set; }

        [JsonProperty("media")]
        public List<Medium> Media { get; set; }
    }
}
