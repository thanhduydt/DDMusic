using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class TopSongOnMonthDetail
    {
        public int Id { get; set; }
        public int Top { get; set; }
        public int IdTopSongOnMonth { get; set; }
        public int IdSong { get; set; }
        [ForeignKey("IdTopSongOnMonth")]
        public virtual TopSongOnMonth TopSongOnMonth { get; set; }
        [ForeignKey("IdSong")]
        public virtual SongModel Song { get; set; }
    }
}
