using Newtonsoft.Json;

namespace GoComics.Shared.Models
{
    //"feature_id": 32,
    //"position": 0,
    //"feature": {
    //    "id": 32,
    //    "title": "Calvin and Hobbes",
    //    "author": "Bill Watterson",
    //    "icon_url": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/ch/tiny_avatar.png",
    //    "icons": {
    //        "large": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/ch/avatar.png",
    //        "mid": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/ch/mid_avatar.png",
    //        "small": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/ch/small_avatar.png",
    //        "tiny": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/ch/tiny_avatar.png"
    //    },
    //    "political_slant": null
    //}

    /// <summary>
    ///
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FeatureBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        [JsonProperty("icons")]
        public ComicIcons Icons { get; set; }

        [JsonProperty("political_slant")]
        public bool? IsPoliticalSlant { get; set; }
    }
}