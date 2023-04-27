using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tp3_serveur.Models
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string PasswordConfirm { get; set; } = null!;
    }
}
