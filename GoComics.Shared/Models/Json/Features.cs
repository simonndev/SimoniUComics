using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoComics.Shared.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Features
    {
        [JsonProperty("feed_name")]
        public string FeedName { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("features")]
        public IList<Feature> FeatureArray { get; set; }
    }
}