using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tp3_serveur.Models;

namespace tp3_serveur.Data
{
    public class tp3_serveurContext : IdentityDbContext<User>
    {
        public tp3_serveurContext (DbContextOptions<tp3_serveurContext> options)
            : base(options){}

        public DbSet<tp3_serveur.Models.Gallery> Gallery { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            User u1 = new User
            {
                Id = "11111111-1111-1111-1111-111111111111",
                UserName = "oui",
                Email = "o@o.o",
                NormalizedEmail = "O@O.O",
                NormalizedUserName = "OUI"
            };
            u1.PasswordHash = hasher.HashPassword(u1, "Salut1!");
            modelBuilder.Entity<User>().HasData(u1);

            modelBuilder.Entity<Gallery>().HasData(
                new { Id = 1, Name = "Galerie du seed", IsPublic = true, UserId = "11111111-1111-1111-1111-111111111111" }
            );
        }

        public DbSet<tp3_serveur.Models.Picture>? Picture { get; set; }
    }
}
