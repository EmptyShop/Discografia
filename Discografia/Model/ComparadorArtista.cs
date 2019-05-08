using System;
using System.Collections.Generic;

namespace Discografia.Model
{
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
