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
        public string Genre { get; set; }
        public int IdSinger { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string IdUser { get; set; }
        public string Lyric { get; set; }
        public string URLImg { get; set; }
        public string URLMusic { get; set; }
        public int CountView { get; set; }
        public bool Accept { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("IdSinger")]
        public virtual SingerModel Singers { get; set; }
        [ForeignKey("IdUser")]
        public virtual UserModel User { get; set; }

        public static List<string> GetAllGerne()
        {
            List<string> gernes = new List<string>()
            {
                "Acoustic","EDM","Pop","Ballad"
            };
            return gernes;
        }

    }

}
