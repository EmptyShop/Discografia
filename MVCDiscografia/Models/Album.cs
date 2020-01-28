using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCDiscografia.Models
{
    public partial class Album
    {
        public int AlbumID { get; set; }

        [Required]
        [StringLength(64)]
        public string Nombre { get; set; }

        [Display(Name="Fecha de Grabación")]
        public DateTime? FechaGrabacion { get; set; }

        [Display(Name="Fecha de Adquisición")]
        public DateTime? FechaAdquisicion { get; set; }

        [Display(Name = "Formato")]
        public int Formato_FormatoID { get; set; }

        [Required]
        [Display(Name = "Formato")]
        public virtual Formato Formato { get; set; }

        public virtual ICollection<CancionAlbums> Tracklist { get; set; }
        public virtual ICollection<Artista> Artistas { get; set; }

        [Display(Name = "Código Discogs")]
        public int? DiscogsReleaseCode { get; set; }

        [Display(Name = "Spotify ID")]
        public string SpotifyID { get; set; }
        
        public Album()
        {
            Tracklist = new HashSet<CancionAlbums>();
            Artistas = new HashSet<Artista>();
        }
    }
}
