using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

using MVCDiscografia.Models;

namespace MVCDiscografia.ViewModels
{
    /* clase wrap para mostrar datos de canción y listas de artistas para asignar */
    public class CancionCreateEditVM
    {
        public Cancion Cancion { get; set; }

        /*Lista de artistas en la BD no asignados a la canción*/
        [Display(Name="Lista de Artistas")]
        public IEnumerable<SelectListItem> ListaArtistas { get; set; }

        /*Lista de artistas asignados a la canción*/
        public IEnumerable<SelectListItem> ListaArtistasCancion { get; set; }

        /*Lista de ids de artistas que el usuario asigna a la canción*/
        [Display(Name="Artistas Seleccionados")]
        public virtual List<int> ArtistasSeleccionados { get; set; }

        public CancionCreateEditVM()
        {
            ArtistasSeleccionados = new List<int>();
        }
    }

    /* clase wrap para mostrar datos de canción y artistas a la vez */
    public class CancionViewVM
    {
        public Cancion Cancion { get; set; }

        [Display(Name = "Artistas")]
        public string losArtistas { get; set; }

        public CancionViewVM()
        {

        }

        public CancionViewVM(Cancion c)
        {
            //obtención de los nombres de los artistas de la canción, separados por comas
            if (c != null)
            {
                Cancion = c;
                losArtistas = c.Artistas.Aggregate<Artista, string, string>(
                    String.Empty, (str, a) => str + a.Nombre + ", ",
                    str => str.Substring(0, str.Length - 2));
            }
        }
    }
}