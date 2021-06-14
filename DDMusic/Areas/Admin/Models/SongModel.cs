using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class SongModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gerne { get; set; }
        public int Singer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Users { get; set; }
        public string Lyric { get; set; }
        public string URLImg { get; set; }
        public string URLMusic { get; set; }
        public int CountView { get; set; }
        public bool Accept { get; set; }
        public bool IsActive { get; set; }
    }
}
