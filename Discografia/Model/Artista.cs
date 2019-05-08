using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Discografia
{
    public class Artista
    {
        public int ArtistaID { get; set; }
        
        [MaxLength(64)]
        [Required]
        public string Nombre { get; set; }

        [MaxLength(32)]
        public string Pais { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<Cancion> Canciones { get; set; }

        public Artista()
        {
            this.Albums = new HashSet<Album>();
            this.Canciones = new HashSet<Cancion>();
        }
    }
}
