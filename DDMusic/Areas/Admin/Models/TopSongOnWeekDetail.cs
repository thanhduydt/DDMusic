using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class TopSongOnWeekDetail
    {
        public int Id { get; set; }
        public int Top { get; set; }
        public int IdSong { get; set; }
        public int IdTopSongOnWeek { get; set; }
        [ForeignKey("IdSong")]
        public virtual SongModel Song { get; set; }
        [ForeignKey("IdTopSongOnWeek")]
        public virtual TopSongOnWeek TopSongOnWeek { get; set; }
    }
}
