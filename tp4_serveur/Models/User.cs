using Microsoft.AspNetCore.Identity;

namespace tp3_serveur.Models
{
    public class User : IdentityUser
    {
        public virtual List<Gallery> Galleries { get; set; } = null!;
    }
}
