using System;
using System.Collections.Generic;

namespace Discografia.Model
{
    class ComparadorCancionAlbums : IEqualityComparer<CancionAlbums>
    {
        public bool Equals(CancionAlbums c1, CancionAlbums c2)
        {
            if (Object.ReferenceEquals(c1, c2)) return true;

            return c1 != null && c2 != null
                && c1.Cancion_CancionID.Equals(c2.Cancion_CancionID)
                && c1.Album_AlbumID.Equals(c2.Album_AlbumID);
        }

        public int GetHashCode(CancionAlbums c)
        {
            int hashCancionID = c.Cancion_CancionID.GetHashCode();
            int hashAlbumID = c.Album_AlbumID.GetHashCode();

            return hashCancionID ^ hashAlbumID;
        }
    }
}

