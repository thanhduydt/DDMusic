using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class TopSongOnMonth
    {
        public int Id { get; set; }
        public DateTime TimeRestart { get; set; }
    }
}
