using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class SongModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gerne { get; set; }
        [ForeignKey("IdSinger")]
        public int IdSinger { get; set; }
        public DateTime ReleaseDate { get; set; }
        [ForeignKey("IdUser")]
        public string IdUser { get; set; }
        public string Lyric { get; set; }
        public string URLImg { get; set; }
        public string URLMusic { get; set; }
        public int CountView { get; set; }
        public bool Accept { get; set; }
        public bool IsActive { get; set; }
        public virtual List<SingerModel> Singers { get; set; }
        public virtual UserModel User { get; set; }
    }
    public class Gerne
    {
        public int Id { get; set; }
        public string GerneName { get; set; }

        public static List<Gerne> GetAllGerne()
        {
            List<Gerne> gernes = new List<Gerne>()
            {
                new Gerne{Id=1,GerneName="Acoustic"},
                new Gerne{Id=2,GerneName="EDM"},
                new Gerne{Id=3, GerneName="Pop"}
            };
            return gernes;
        }

    }

}
