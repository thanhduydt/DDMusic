using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class EditUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber{get;set;}
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public string URLImg { get; set; }
        public string Gender { get; set; }
    }
}
