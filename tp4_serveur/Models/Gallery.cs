
using System.Text.Json.Serialization;

namespace tp3_serveur.Models
{
    public class Gallery
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual List<Picture> Pictures { get; set; } = null!;
        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
