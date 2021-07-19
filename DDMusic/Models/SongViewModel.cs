using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Models
{
    public class SongViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string img { get; set; }
        public string lyrics { get; set; }
        public string src { get; set; }

        internal object ToListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
