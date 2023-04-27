using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp3_serveur.Data;
using tp3_serveur.Models;

namespace tp3_serveur.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GalleriesController : ControllerBase
    {
        private readonly tp3_serveurContext _context;

        public GalleriesController(tp3_serveurContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Gallery>>> GetPublicGalleries()
        {
            // Si un utilisateur est authentifié, le trouver
            User? user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Obtenir toutes les galeries publiques
            List<Gallery> galleries = await _context.Gallery.ToListAsync();

            // Retirer les galeries de l'utilisateur authentifié s'il y en a un
            if(user != null)
            {
                galleries = galleries.Where(x => x.User == null || !x.User.Equals(user)).ToList();
            }
            return galleries;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gallery>>> GetMyGalleries()
        {
            User? user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound();
            }

            return user.Galleries;
        }

        [HttpPost]
        public async Task<ActionResult<Gallery>> PostGallery(Gallery gallery)
        {
            User? user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound();
            }

            // Lien entre user et gallery
            user.Galleries.Add(gallery);
            gallery.User = user;

            // Ajout dans la BD
            _context.Gallery.Add(gallery);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostGallery", new { id = gallery.Id }, gallery);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGallery(int id)
        {
            User? user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Gallery? gallery = await _context.Gallery.FindAsync(id);
            if (user == null || gallery == null)
            {
                return NotFound();
            }

            // L'utilisateur est-il propriétaire de la galerie ?
            if (!user.Galleries.Contains(gallery))
            {
                return Unauthorized();
            }

            // Supprimer la galerie
            _context.Gallery.Remove(gallery);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Gallerie supprimée !" });
        }
    }
}
