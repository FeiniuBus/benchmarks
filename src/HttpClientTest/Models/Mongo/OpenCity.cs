using System.ComponentModel.DataAnnotations;
using FeiniuBus;
using FeiniuBus.MongoDB;
using Newtonsoft.Json;

namespace HttpClientTestCore.Models.Mongo
{
    [CollectionName("opend_city")]
    public class OpenCity : Entity
    {
        [Required]
        public string Adcode { get; set; }

        [Required]
        [JsonProperty("city_name")]
        public string CityName { get; set; }

        [Required]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [Required]
        [JsonProperty("site_level")]
        public int SiteLevel { get; set; }

        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }

        [Required]
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [Required]
        public string Tag { get; set; }
    }
}