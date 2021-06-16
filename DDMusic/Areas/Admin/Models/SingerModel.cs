using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class SingerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Country { get; set; }

        //Danh sách quốc gia
        public static List<string> GetCountry()
        {
            List<string> Countries = new List<string>()
            {
                "Việt Nam","Hàn Quốc","Âu Mỹ"
            };
            return Countries;
        }
    }
}
