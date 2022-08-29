using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG.Blazor.Identity.Models
{
    public class ResetPasswordModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string code { get; set; }
    }
}
