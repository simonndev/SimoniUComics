using Newtonsoft.Json;
using System;

namespace GoComics.Shared.Models
{
    //"id": 1428,
    //"author": "Eric Scott",
    //"title": "1 and Done",
    //"feature_code": "oad",
    //"sort_title": "1 and done",
    //"category": "comic",
    //"language": "english",
    //"permalink_title": "1-and-done",
    //"gocomics_link": "http://www.gocomics.com/1-and-done",
    //"icon_url": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/oad/tiny_avatar.png",
    //"icons": {
    //    "large": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/oad/avatar.png",
    //    "mid": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/oad/mid_avatar.png",
    //    "small": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/oad/small_avatar.png",
    //    "tiny": "http://avatar.amuniversal.com/feature_avatars/avatars_large/features/oad/tiny_avatar.png"
    //},
    //"start_date": "2015-08-16",
    //"created_at": "2015-08-14T10:44:28-05:00",
    //"political_slant": null
    [JsonObject(MemberSerialization.OptIn)]
    public class Feature : FeatureBase
    {
        [JsonProperty("sort_title")]
        public string SortTitle { get; set; }

        [JsonProperty("permalink_title")]
        public string PermalinkTitle { get; set; }

        [JsonProperty("feature_code")]
        public string FeatureCode { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("gocomics_link")]
        public string GoComicsLink { get; set; }
        
        [JsonProperty("start_date")]
        public DateTime StartedDate { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}