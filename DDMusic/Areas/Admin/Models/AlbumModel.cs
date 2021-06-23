using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class AlbumModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int QuantitySong { get; set; }
        public int IdSinger { get; set; }
        [ForeignKey("IdSinger")]
        public virtual SingerModel Singer { get; set; }
    }
}
