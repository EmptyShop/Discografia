using System;
using System.Collections.Generic;

namespace Discografia.Model
{
    class ComparadorCancion : IEqualityComparer<Cancion>
    {
        public bool Equals(Cancion c1, Cancion c2)
        {
            if (Object.ReferenceEquals(c1, c2)) return true;

            return c1 != null && c2 != null && c1.CancionID.Equals(c2.CancionID);
        }

        public int GetHashCode(Cancion c)
        {
            int hashCancionID = c.CancionID.GetHashCode();
            int hashCancionNombre = c.Nombre == null ? 0 : c.Nombre.GetHashCode();

            return hashCancionID ^ hashCancionNombre;
        }
    }
}
