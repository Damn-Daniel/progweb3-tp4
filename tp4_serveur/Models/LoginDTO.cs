using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp3_serveur.Models
{
    public class LoginDTO
    {
        [Required]
        public String Username { get; set; } = null!;
        [Required]
        public String Password { get; set; } = null!;
    }
}
