using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Discografia
{
    public class Cancion
    {
        public int CancionID { get; set; }

        [MaxLength(128)]
        [Required]
        public string Nombre { get; set; }
        public TimeSpan? Duracion { get; set; }

        [Range(1900,2100)]
        public short? Año { get; set; }

        public virtual ICollection<Artista> Artistas { get; set; }
        public virtual ICollection<CancionAlbums> CancionAlbums { get; set; }

        public Cancion()
        {
            this.Artistas = new HashSet<Artista>();
            this.CancionAlbums = new HashSet<CancionAlbums>();
        }

        //genera una lista de canciones (ID, nombre, artistas)
        public static object CatalogoCanciones()
        {
            try
            {
                var contexto = new DiscografiaDB();
                var setCanciones = contexto.Canciones.AsEnumerable().Select(s =>
                        new
                        {
                            s.CancionID,
                            s.Nombre,
                            losArtistas = s.Artistas.Aggregate<Artista, string, string>(
                            String.Empty, (str, a) => str + a.Nombre + ", ",
                            str => str.Substring(0, str.Length - 2))
                        }
                        ).OrderBy(s => s.losArtistas).ThenBy(s => s.Nombre).ToList();
                return setCanciones;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
