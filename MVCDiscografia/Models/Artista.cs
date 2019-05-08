using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCDiscografia.Models
{
    public partial class Artista
    {
        public int ArtistaID { get; set; }

        [Required]
        [StringLength(64)]
        public string Nombre { get; set; }

        [StringLength(32)]
        [Display(Name="País")]
        public string Pais { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<Cancion> Canciones { get; set; }

        public Artista()
        {
            Albums = new HashSet<Album>();
            Canciones = new HashSet<Cancion>();
        }
    }

    public class ComparadorArtista : IEqualityComparer<Artista>
    {
        public bool Equals(Artista a1, Artista a2)
        {
            if (Object.ReferenceEquals(a1, a2)) return true;

            return a1 != null && a2 != null && a1.ArtistaID.Equals(a2.ArtistaID);
        }

        public int GetHashCode(Artista a)
        {
            int hashArtistaID = a.ArtistaID.GetHashCode();
            int hashArtistaNombre = a.Nombre == null ? 0 : a.Nombre.GetHashCode();

            return hashArtistaID ^ hashArtistaNombre;
        }
    }
}
