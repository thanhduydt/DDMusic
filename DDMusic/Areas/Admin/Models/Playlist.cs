using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual UserModel User { get; set; }
    }
}
