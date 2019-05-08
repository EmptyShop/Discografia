using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDiscografia.Models
{
    public partial class CancionAlbums
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Cancion_CancionID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Album_AlbumID { get; set; }

        public short? Posicion { get; set; }

        public virtual Album Album { get; set; }
        public virtual Cancion Cancion { get; set; }

        public CancionAlbums()
        {

        }
    }

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
