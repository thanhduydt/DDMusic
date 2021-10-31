using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class ViewSongOfDayDetail
    {
        public int Id { get; set; }
        public int? IdSong { get; set; }
        public int IdViewSongOfDay { get; set; }
        public int CountView { get; set; }
        [ForeignKey("IdSong")]
        public virtual SongModel Song { get; set; }
        [ForeignKey("IdViewSongOfDay")]
        public virtual ViewSongOfDay ViewSongOfDay{ get; set; }
    }
}
