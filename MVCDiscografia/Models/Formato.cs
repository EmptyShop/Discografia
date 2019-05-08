using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDiscografia.Models
{
    [Table("Formatos")]
    public partial class Formato
    {
        public int FormatoID { get; set; }

        [Required]
        [StringLength(16)]
        [Display(Name="Descripción")]
        public string Descripcion { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
        
        public Formato()
        {
            Albums = new HashSet<Album>();
        }
    }
}
