using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Discografia
{
    public class Album
    {
        public int AlbumID { get; set; }

        [MaxLength(64)]
        [Required]
        public string Nombre { get; set; }

        [Required]
        public virtual Formato Formato { get; set; }
        
        public DateTime? FechaGrabacion { get; set; }
        public DateTime? FechaAdquisicion { get; set; }

        public virtual ICollection<Artista> Artistas { get; set; }
        public virtual ICollection<CancionAlbums> Tracklist { get; set; }

        public int? DiscogsReleaseCode { get; set; }
        public string SpotifyID { get; set; }

        public Album()
        {
            this.Artistas = new HashSet<Artista>();
            this.Tracklist = new HashSet<CancionAlbums>();
        }

        //genera una lista de álbumes (ID, nombre, artistas)
        public static object CatalogoAlbumes()
        {
            try
            {
                var contexto = new DiscografiaDB();
                var setAlbums = contexto.Albums.AsEnumerable().Select(s =>
                    new
                    {
                        s.AlbumID,
                        s.Nombre,
                        losArtistas = s.Artistas.Aggregate<Artista, string, string>(
                        String.Empty, (str, a) => str + a.Nombre + ", ",
                        str => str.Substring(0, str.Length - 2))
                    }
                    ).OrderBy(s => s.losArtistas).ThenBy(s => s.Nombre).ToList();
                return setAlbums;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}
