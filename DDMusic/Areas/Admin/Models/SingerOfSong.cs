using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class SingerOfSong
    {
        public int Id { get; set; }
        public int IdSinger { get; set; }
        public int IdSong { get; set; }
        [ForeignKey("IdSong")]
        public virtual SongModel Song { get; set; }
        [ForeignKey("IdSinger")]
        public virtual SingerModel Singer { get; set; }
    }
}
