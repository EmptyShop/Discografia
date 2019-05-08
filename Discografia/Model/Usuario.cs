using System.ComponentModel.DataAnnotations;

namespace Discografia
{
    public class Usuario
    {
        public int UsuarioID { get; set; }

        [Required]
        [MaxLength(16)]
        public string User { get; set; }

        [Required]
        [MaxLength(64)]
        public string Password { get; set; }

        [Required]
        public short Estatus { get; set; }

        public Usuario()
        {

        }
    }
}
