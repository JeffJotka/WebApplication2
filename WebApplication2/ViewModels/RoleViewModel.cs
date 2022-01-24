using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class RoleViewModel 
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
    }
}
