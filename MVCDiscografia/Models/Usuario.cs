using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDiscografia.Models
{
    [Table("Usuarios")]
    public partial class Usuario
    {
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(16)]
        [Display(Name="Usuario")]
        public string User { get; set; }

        [Required(ErrorMessage="Proporciona la contraseña")]
        [StringLength(64)]
        [Display(Name="Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage="La contraseña no es la misma.")]
        [StringLength(64)]
        [Display(Name="Confirma")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

        public short Estatus { get; set; }

        public Usuario()
        {

        }
    }
}
