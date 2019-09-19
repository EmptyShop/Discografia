using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

using MVCDiscografia.Models;

namespace MVCDiscografia.ViewModels
{
    public class AlbumCreateEditVM
    {
        public Album Album { get; set; }

        [Display(Name = "Lista de Artistas")]
        public IEnumerable<SelectListItem> ListaArtistas { get; set; }

        public IEnumerable<SelectListItem> ListaArtistasAlbum { get; set; }

        [Display(Name = "Artistas Seleccionados")]
        public virtual List<int> ArtistasSeleccionados { get; set; }

        [Display(Name = "Lista de Canciones")]
        public IEnumerable<SelectListItem> ListaCanciones { get; set; }

        public IEnumerable<SelectListItem> Tracklist { get; set; }

        [Display(Name = "Canciones Seleccionados")]
        public virtual List<int> CancionesSeleccionadas { get; set; }

        public AlbumCreateEditVM()
        {
            ArtistasSeleccionados = new List<int>();
            CancionesSeleccionadas = new List<int>();
        }
    }

    public class AlbumViewVM
    {
        public Album Album { get; set; }

        [Display(Name = "Artistas")]
        public string losArtistas { get; set; }

        public IEnumerable<TracklistView> elTracklist { get; set; }

        public DiscogsView datosDiscogs { get; set; }

        public AlbumViewVM()
        {

        }

        public AlbumViewVM(Album a)
        {
            //obtención de los nombres de los artistas del album, separados por comas
            if (a != null)
            {
                Album = a;
                losArtistas = a.Artistas.Aggregate<Artista, string, string>(
                    String.Empty, (str, ar) => str + ar.Nombre + ", ",
                    str => str.Substring(0, str.Length - 2));

                elTracklist = a.Tracklist.Select(t => new TracklistView()
                {
                    posicion = t.Posicion,
                    nombreCancion = t.Cancion.Nombre
                }).OrderBy(t => t.posicion);
               
            }
        }

        public class TracklistView
        {
            public short? posicion { get; set; }
            public string nombreCancion { get; set; }
        }

        public class DiscogsView
        {
            [JsonProperty("thumb")]
            public string coverUrl { get; set; }

            [Display(Name = "Disquera")]
            public string disquera { get; set; }

            [Display(Name = "Núm Catálogo")]
            public string numCatalogo { get; set; }
        }
    }

    public class AlbumCancionesSeleccionadas
    {
        public string CancionIDPosicion { get; set; }
        public string Descripcion { get; set; }
    }
}