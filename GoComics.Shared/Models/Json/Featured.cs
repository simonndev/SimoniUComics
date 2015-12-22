using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GoComics.Shared.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Featured
    {
        [JsonProperty("feature_id")]
        public int FeatureId { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("feature")]
        public FeatureBase Feature { get; set; }
    }
}
