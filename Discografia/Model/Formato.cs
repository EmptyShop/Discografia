using System.ComponentModel.DataAnnotations;

namespace Discografia
{
    public class Formato
    {
        public int FormatoID { get; set; }

        [MaxLength(16)]
        [Required]
        public string Descripcion { get; set; }

        public Formato()
        {

        }

        //para búsquedas y comparaciones en el combo de formatos en la sección de álbumes
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Formato)) return false;

            Formato f = (Formato)obj;

            return this.FormatoID.Equals(f.FormatoID);
        }

        public override int GetHashCode()
        {
            return this.FormatoID.GetHashCode();
        }
    }
}
