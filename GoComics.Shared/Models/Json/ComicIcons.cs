using Newtonsoft.Json;

namespace GoComics.Shared.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ComicIcons
    {
        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("mid")]
        public string Medium { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("tiny")]
        public string Tiny { get; set; }
    }
}