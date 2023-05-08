using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp3_serveur.Data;
using tp3_serveur.Models;

namespace tp3_serveur.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PicturesController : Controller
    {
        private readonly tp3_serveurContext _context;

        public PicturesController(tp3_serveurContext context)
        {
            _context = context;
        }
        #region get picture
        // GET: Pictures/Details/5
        [HttpGet("{size}/{id}")]
        public async Task<ActionResult> GetFile(string size, int id)
        {
            if (_context.Picture == null)
            {
                return NotFound();
            }

            Picture? picture = await _context.Picture.FindAsync(id);
            if (picture == null || picture.FileName == null || picture.MimeType == null)
            {
                return NotFound(new { Message = "Cet image n'existe pas ou n'a pas de photo." });
            }
            if (!(Regex.Match(size, "lg|sm").Success))
            {
                return BadRequest(new { Message = "La taille demandé est inadéquate." });
            }
            byte[] bytes = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "/images/" + size + "/" + picture.FileName);
            return File(bytes, picture.MimeType);
        }
        #endregion

        #region post picture fonctionne
        [HttpPost("{id}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<Picture>> PostPicture(int id)
        {
            Gallery? gallerie = await _context.Gallery.FindAsync(id);
            if (gallerie==null)
            {
                return NotFound(new { Message = "Cette gallerie n'existe pas." });
            }

            var picture = new Picture();

            try
            {
                IFormCollection formCollection = await Request.ReadFormAsync();
                IFormFile? file = formCollection.Files.GetFile("monImage");
                if (file != null)
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
                    picture.Gallerie = gallerie;

                    gallerie.Pictures.Add(picture);

                    _context.Picture.Add(picture);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound(new { Message = "Aucune image fournie" });
                }
            }
            catch (Exception)
            {

                throw;
            }


            return Ok(new { Id = picture.Id });
        }
        #endregion

        // GET: Pictures/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicture(int id)
        {
            if (_context.Picture==null)
            {
                return NotFound();
            }
            User? user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound();
            }

            var picture = await _context.Picture.FindAsync(id);
            if (picture == null)
            {
                return NotFound(new { Message = "Cet photo n'existe pas" });
            }
            if (picture.MimeType!=null&&picture.FileName!=null)
            {
                System.IO.File.Delete(Directory.GetCurrentDirectory() + "/images/lg/" + picture.FileName);
                System.IO.File.Delete(Directory.GetCurrentDirectory() + "/images/sm/" + picture.FileName);
            }
            Gallery? gal = await _context.Gallery.FindAsync(picture.Gallerie.Id);
            gal.Pictures.Remove(picture);
            _context.Picture.Remove(picture);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
