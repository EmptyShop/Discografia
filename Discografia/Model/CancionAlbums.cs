using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Discografia
{
    public class CancionAlbums
    {
        [Column(Order=0),Key,ForeignKey("Cancion")]
        public int Cancion_CancionID { get; set; }

        [Column(Order=1),Key,ForeignKey("Album")]
        public int Album_AlbumID { get; set; }

        public short? Posicion { get; set; }

        public virtual Album Album { get; set; }
        public virtual Cancion Cancion { get; set; }

        public CancionAlbums()
        {

        }
    }
}
