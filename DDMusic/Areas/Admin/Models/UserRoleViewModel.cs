using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class UserRoleViewModel
    {
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        public List<Role> ListRole { get; set; }
    }
    public class Role
    {
        public string IdRole { get; set; }
        public string NameRole { get; set; }
        public bool IsSelected { get; set; }
    }
}
