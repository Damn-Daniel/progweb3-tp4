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

        #region post gallery
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<Gallery>> PostGallery()
        {
            #region créé picture


            var picture = new Picture();
            IFormCollection formCollection = await Request.ReadFormAsync();
            IFormFile? file = formCollection.Files.GetFile("monImage");
            if (file!=null)
            {
                try
                {
                    Image image = Image.Load(file.OpenReadStream());
                    picture.FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    picture.MimeType = file.ContentType;

                    image.Save(Directory.GetCurrentDirectory() + "/images/lg/" + picture.FileName);
                    image.Mutate(i =>
                        i.Resize(new ResizeOptions()
                        {
                            Mode = ResizeMode.Min,
                            Size = new Size() { Width = 320 }
                        })
                    );
                    image.Save(Directory.GetCurrentDirectory() + "/images/sm/" + picture.FileName);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                picture = null;
            }
            
                
            #endregion

            #region identifie user
            User? user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound();
            }
            #endregion

            #region create gallery gal with user and picture

            //Obtient le nom de la gallerie
            string? galleriename = Request.Form["gallery"];
            if (galleriename == null)
            {
                return BadRequest(new { Message = "Aucun nom fourni pour la gallery" });
            }

            Gallery gal = new Gallery();
            gal.Name = galleriename;
            gal.Id = 0;
            gal.User = user;
            if (picture!=null)
            {
                gal.Pictures = new List<Picture>() { picture};
            }
            

            #endregion

            if (picture!=null)
            {
                picture.Gallerie = gal;
                _context.Picture.Add(picture);
            }
            
            user.Galleries.Add(gal);
            _context.Gallery.Add(gal);
            
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostGallery", new { id = gal.Id }, gal);
        }
        #endregion

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
            //Supprimer les photos de la gallerie
            #region supprime photo
            if (_context.Picture != null)
            {
                var picture = gallery.Pictures.ToList();
                if (picture!=null)
                {
                    foreach (var item in picture)
                    {
                        if (item.MimeType != null && item.FileName != null)
                        {
                            System.IO.File.Delete(Directory.GetCurrentDirectory() + "/images/lg/" + item.FileName);
                            System.IO.File.Delete(Directory.GetCurrentDirectory() + "/images/sm/" + item.FileName);
                        }
                        _context.Picture.Remove(item);
                        gallery.Pictures.Remove(item);
                    }
                }
            }
            await _context.SaveChangesAsync();
            #endregion

            // Supprimer la galerie
            _context.Gallery.Remove(gallery);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Gallerie supprimée !" });
        }
    }
}
