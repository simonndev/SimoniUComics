using Newtonsoft.Json;
using System;

namespace GoComics.Shared.Models
{
    //{
    //    "id": 1375870,
    //    "title": "Garfield",
    //    "author": "Jim Davis",
    //    "rights": "© Paws, Inc.",
    //    "gocomics_link": "http://www.gocomics.com/garfield/2015/11/29",
    //    "feature_link": "http://www.gocomics.com/garfield",
    //    "feature_id": 72,
    //    "feature_code": "ga",
    //    "sort_name": "garfield",
    //    "current_link": "",
    //    "previous_link": "http://www.gocomics.com/api/feature/1375564/item",
    //    "next_link": "",
    //    "image_link": "http://assets.amuniversal.com/7b9ea9f059a8013311fa005056a9545d",
    //    "webready_link": "http://assets.amuniversal.com/7a81e20059a8013311fa005056a9545d",
    //    "image_width": 900,
    //    "image_height": 633,
    //    "display_date": "November 29, 2015",
    //    "publish_date": "2015-11-29",
    //    "updated": "2015-10-20T17:33:15-05:00"
    //}
    [JsonObject(MemberSerialization.OptIn)]
    public class FeatureItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("rights")]
        public string Copyrights { get; set; }

        [JsonProperty("gocomics_link")]
        public string GoComicsLink { get; set; }

        [JsonProperty("feature_link")]
        public string FeatureLink { get; set; }

        [JsonProperty("feature_id")]
        public int FeatureId { get; set; }

        [JsonProperty("feature_code")]
        public string FeatureCode { get; set; }

        [JsonProperty("sort_name")]
        public string ShortName { get; set; }

        [JsonProperty("current_link")]
        public string CurrentLink { get; set; }

        [JsonProperty("previous_link")]
        public string PreviousLink { get; set; }

        [JsonProperty("next_link")]
        public string NextLink { get; set; }

        [JsonProperty("image_link")]
        public string ImageLink { get; set; }

        [JsonProperty("webready_link")]
        public string WebreadyLink { get; set; }

        [JsonProperty("image_width")]
        public int ImageWidth { get; set; }

        [JsonProperty("image_height")]
        public int ImageHeight { get; set; }

        [JsonProperty("display_date")]
        public string DisplayDateString { get; set; }

        [JsonProperty("publish_date")]
        public string PublishDateString { get; set; }

        [JsonProperty("updated")]
        public DateTime UpdatedDate { get; set; }
    }
}