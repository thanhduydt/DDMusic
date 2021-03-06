using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class PlaylistDetail
    {
        public int Id { get; set; }
        public int IdSong { get; set; }
        public int IdPlaylist { get; set; }
        [ForeignKey("IdSong")]
        public virtual SongModel Song { get; set; }
        [ForeignKey("IdPlaylist")]
        public virtual Playlist PlayList { get; set; }
    }
}
