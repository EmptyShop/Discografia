using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MVCDiscografia.Models
{
    [Table("Canciones")]
    [Bind(Include = "CancionID,Nombre,Duracion,Año,Artistas")]
    public partial class Cancion
    {
        public int CancionID { get; set; }

        [Required]
        [StringLength(128)]
        public string Nombre { get; set; }

        [RegularExpression(@"^(\d{1,2}:)?[0-5]?\d:[0-5]\d$", ErrorMessage = "El formato de duración es hh:MM:SS")]
        [Display(Name = "Duración")]
        public TimeSpan? Duracion { get; set; }

        [Range(1900,2100)]
        public short? Año { get; set; }

        public virtual ICollection<CancionAlbums> CancionAlbums { get; set; }
        public virtual ICollection<Artista> Artistas { get; set; }

        public Cancion()
        {
            CancionAlbums = new HashSet<CancionAlbums>();
            Artistas = new HashSet<Artista>();
        }
    }
}
