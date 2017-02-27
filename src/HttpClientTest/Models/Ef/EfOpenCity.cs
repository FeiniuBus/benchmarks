using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HttpClientTestCore.Models.Ef
{
    [Table("base_opend_city")]
    public class EfOpenCity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Adcode { get; set; }

        public string CityName { get; set; }
        public string CompanyName { get; set; }
        public int SiteLevel { get; set; }
        public string ProvinceCode { get; set; }

        [Column("Disabled")]
        public bool Disabled { get; set; }

        public bool Deleted { get; set; }
        public string Tag { get; set; }
    }
}