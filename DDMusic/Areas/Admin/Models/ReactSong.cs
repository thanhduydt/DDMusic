using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class ReactSong
    {
        public int Id { get; set; }
        public int IdSong { get; set; }
        public string IdUser { get; set; }
        public bool React { get; set; }
        [ForeignKey("IdUser")]
        public virtual UserModel User { get; set; }
        [ForeignKey("IdSong")]
        public virtual SongModel Song { get; set; }
    }
}
