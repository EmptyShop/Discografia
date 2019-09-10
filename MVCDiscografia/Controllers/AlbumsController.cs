using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVCDiscografia.Models;
using MVCDiscografia.ViewModels;
using System.Globalization;

namespace MVCDiscografia.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private DiscografiaDB db = new DiscografiaDB();
        private TextInfo TICaseFormat;
        private string formatoArtistaCancion = "{0} --- {1}";

        // GET: Albums
        public ActionResult Index()
        {
            ICollection<AlbumViewVM> losAlbums = new HashSet<AlbumViewVM>();

            //la lista de albums con artistas incluídos
            db.Albums.ToList().ForEach(a => losAlbums.Add(new AlbumViewVM(a)));

            return View(losAlbums.OrderBy(a => a.losArtistas).ThenBy(a => a.Album.Nombre));
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            //detalle del album con artistas y tracklist incluídos
            AlbumViewVM albumVM = new AlbumViewVM(album);
            return View(albumVM);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            //creamos las listas de formatos, artistas y canciones y las enviamos a la vista a través de ViewBag
            ViewBag.Formato_FormatoID = new SelectList(db.Formatos.OrderBy(f => f.Descripcion), "FormatoID", "Descripcion");
            ViewBag.listaArtistas = new SelectList(db.Artistas.OrderBy(a => a.Nombre), "ArtistaID", "Nombre");
            //listaArtistasSeleccionados va vacía siempre porque es donde el usuaro asignará artistas
            ViewBag.listaArtistasSeleccionados = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
            ViewBag.listaCanciones = new SelectList(
                db.Canciones.AsEnumerable().OrderBy(c => c.Artistas.FirstOrDefault().Nombre).ThenBy(c => c.Nombre)
                .Select(c => new
                {
                    CancionID = c.CancionID,
                    Descripcion = string.Format(formatoArtistaCancion, c.Artistas.FirstOrDefault().Nombre, c.Nombre)
                }), "CancionID", "Descripcion");
            //listaCancionesSeleccionadas va vacía siempre porque es donde el usuario crerará el tracklist
            ViewBag.listaCancionesSeleccionadas = new SelectList(new List<SelectList>(), "CancionIDPosicion", "Descripcion");

            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "AlbumID,Nombre,FechaGrabacion,FechaAdquisicion,Formato_FormatoID,Artistas,DiscogsReleaseCode")] Album album,
            List<int> listaArtistasSeleccionados, List<string> listaCancionesSeleccionadas)
        {
            ICollection<Artista> artistasSeleccionados = new HashSet<Artista>();
            ICollection<Cancion> cancionesSeleccionadas = new HashSet<Cancion>();
            ICollection<AlbumCancionesSeleccionadas> cancionesSeleccionadasSelectList = new HashSet<AlbumCancionesSeleccionadas>();

            TICaseFormat = new CultureInfo("es-MX", false).TextInfo;

            //cuando el álbum no contiene tracklist, inicializamos la lista de canciones para evitar errores
            if (listaCancionesSeleccionadas == null)
            {
                listaCancionesSeleccionadas = new List<string>();
            }

            try
            {
                /*debido a que listaCancionesSeleccionadas es una lista de valores "id-posicion", creamos una lista 
                 * que contenga las canciones seleccionadas */
                listaCancionesSeleccionadas.ForEach(t =>
                {
                    string[] arrData = t.Split('-');    //cada item contiene el valor "id-posición"
                    //con el primer item (id) buscamos la canción
                    Cancion cancion = db.Canciones.Find(Convert.ToInt32(arrData[0]));
                    cancionesSeleccionadas.Add(cancion);

                    /*creamos una lista compatible con SelectList y la llenamos con los valores necesarios.
                     * Esta lista sólo la usaremos para asignarla a ViewBag en el caso de que el ModelState sea rechazado */
                    cancionesSeleccionadasSelectList.Add(new AlbumCancionesSeleccionadas()
                    {
                        CancionIDPosicion = t,
                        Descripcion = string.Format(formatoArtistaCancion, cancion.Artistas.FirstOrDefault().Nombre, cancion.Nombre)
                    });
                });

                //el álbum debe tener al menos un artista asignado
                if (listaArtistasSeleccionados == null)
                {
                    ModelState.AddModelError("", "Asigna al álbum por lo menos un artista.");
                }
                else
                {
                    //asignamos los artistas a la lista de artistas obteniéndolos de la lista de ids de artistas seleccionados
                    listaArtistasSeleccionados.ForEach(a => artistasSeleccionados.Add(db.Artistas.Find(a)));
                    album.Artistas = artistasSeleccionados;

                    if (album.Formato_FormatoID > 0)
                    {
                        /*el modelo requiere que se asigne un formato y al no recibirlo de la vista
                         * genera un error de modelo. Como sí tenemos el id del formato, buscamos el formato
                         * y lo asignamos al álbum y posteriormente eliminamos el error de modelo */
                        album.Formato = db.Formatos.Find(album.Formato_FormatoID);
                        ModelState["Formato"].Errors.Clear();
                        UpdateModel(album);
                    }

                    if (ModelState.IsValid)
                    {
                        //validación de album duplicado en la BD
                        if (!db.Albums.AsEnumerable().Any(s =>
                            s.Nombre.ToLower() == album.Nombre.Trim().ToLower()
                                && s.Artistas.Count == artistasSeleccionados.Count
                                && !s.Artistas.Except(artistasSeleccionados, new Models.ComparadorArtista())
                                    .Any()
                                && s.Formato.FormatoID == album.Formato_FormatoID))
                        {
                            album.Nombre = TICaseFormat.ToTitleCase(album.Nombre.Trim());

                            db.Albums.Add(album);
                            db.SaveChanges();

                            //conformación del tracklist
                            if (listaCancionesSeleccionadas.Count > 0)
                            {
                                //de la lista de valores "id-posición" construimos el tracklist
                                listaCancionesSeleccionadas.ForEach(t =>
                                {
                                    CancionAlbums albumTracklist = new CancionAlbums();

                                    string[] arrData = t.Split('-');
                                    albumTracklist.Cancion = db.Canciones.Find(Convert.ToInt32(arrData[0]));    //id
                                    albumTracklist.Posicion = Convert.ToInt16(arrData[1]);  //posición
                                    album.Tracklist.Add(albumTracklist);
                                });
                                db.SaveChanges();
                            }

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "El álbum ya está registrado en la BD.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al agregar el álbum. " + e.Message);
            }

            //volvemos a llenar las listas de artistas y canciones para regresar la info. al usuario
            try
            {
                ViewBag.Formato_FormatoID = new SelectList(db.Formatos.OrderBy(f => f.Descripcion), "FormatoID", "Descripcion", album.Formato_FormatoID);
                ViewBag.listaArtistas = new SelectList(db.Artistas.AsEnumerable().Except(artistasSeleccionados).OrderBy(a => a.Nombre), "ArtistaID", "Nombre");
                ViewBag.listaArtistasSeleccionados = new SelectList(artistasSeleccionados, "ArtistaID", "Nombre");
                ViewBag.listaCanciones = new SelectList(
                    db.Canciones.AsEnumerable().Except(cancionesSeleccionadas).OrderBy(c => c.Artistas.FirstOrDefault().Nombre).ThenBy(c => c.Nombre)
                    .Select(c => new
                    {
                        CancionID = c.CancionID,
                        Descripcion = string.Format(formatoArtistaCancion, c.Artistas.FirstOrDefault().Nombre, c.Nombre)
                    }), "CancionID", "Descripcion");
                ViewBag.listaCancionesSeleccionadas = new SelectList(cancionesSeleccionadasSelectList, "CancionIDPosicion", "Descripcion");
            }
            catch (Exception)
            {
                if (ViewBag.Formato_FormatoID == null)
                {
                    ViewBag.Formato_FormatoID = new SelectList(new List<SelectList>(), "FormatoID", "Descripcion");
                }
                if (ViewBag.listaArtistas == null)
                {
                    ViewBag.listaArtistas = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
                }
                if (ViewBag.listaArtistasSeleccionados == null)
                {
                    ViewBag.listaArtistasSeleccionados = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
                }
                if (ViewBag.listaCanciones == null)
                {
                    ViewBag.listaCanciones = new SelectList(new List<SelectList>(), "CancionID", "Descripcion");
                }
                if (ViewBag.listaCancionesSeleccionadas == null)
                {
                    ViewBag.listaCancionesSeleccionadas = new SelectList(new List<SelectList>(), "CancionIDPosicion", "Descripcion");
                }
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            try
            {
                ICollection<Cancion> cancionesSeleccionadas = new HashSet<Cancion>();
                album.Tracklist.ToList().ForEach(t => cancionesSeleccionadas.Add(t.Cancion));

                ViewBag.Formato_FormatoID = new SelectList(db.Formatos.OrderBy(f => f.Descripcion), "FormatoID", "Descripcion", album.Formato_FormatoID);
                ViewBag.listaArtistas = new SelectList(db.Artistas.AsEnumerable().Except(album.Artistas).OrderBy(a => a.Nombre), "ArtistaID", "Nombre");
                ViewBag.listaArtistasSeleccionados = new SelectList(album.Artistas, "ArtistaID", "Nombre");
                ViewBag.listaCanciones = new SelectList(
                    db.Canciones.AsEnumerable().Except(cancionesSeleccionadas).OrderBy(c => c.Artistas.FirstOrDefault().Nombre).ThenBy(c => c.Nombre)
                    .Select(c => new
                    {
                        CancionID = c.CancionID,
                        Descripcion = string.Format(formatoArtistaCancion, c.Artistas.FirstOrDefault().Nombre, c.Nombre)
                    }), "CancionID", "Descripcion");
                ViewBag.listaCancionesSeleccionadas = new SelectList(album.Tracklist.OrderBy(t => t.Posicion)
                    .Select(t => new
                    {
                        CancionIDPosicion = t.Cancion_CancionID.ToString() + '-' + t.Posicion,
                        Descripcion = string.Format(formatoArtistaCancion, t.Cancion.Artistas.FirstOrDefault().Nombre, t.Cancion.Nombre)
                    }), "CancionIDPosicion", "Descripcion");
            }
            catch (Exception)
            {
                if (ViewBag.Formato_FormatoID == null)
                {
                    ViewBag.Formato_FormatoID = new SelectList(new List<SelectList>(), "FormatoID", "Descripcion");
                }
                if (ViewBag.listaArtistas == null)
                {
                    ViewBag.listaArtistas = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
                }
                if (ViewBag.listaArtistasSeleccionados == null)
                {
                    ViewBag.listaArtistasSeleccionados = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
                }
                if (ViewBag.listaCanciones == null)
                {
                    ViewBag.listaCanciones = new SelectList(new List<SelectList>(), "CancionID", "Descripcion");
                }
                if (ViewBag.listaCancionesSeleccionadas == null)
                {
                    ViewBag.listaCancionesSeleccionadas = new SelectList(new List<SelectList>(), "CancionIDPosicion", "Descripcion");
                }
            }

            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "AlbumID,Nombre,FechaGrabacion,FechaAdquisicion,Formato_FormatoID,DiscogsReleaseCode")] Album album,
            List<int> listaArtistasSeleccionados, List<string> listaCancionesSeleccionadas)
        {
            ICollection<Artista> artistasSeleccionados = new HashSet<Artista>();
            ICollection<Cancion> cancionesSeleccionadas = new HashSet<Cancion>();
            ICollection<AlbumCancionesSeleccionadas> cancionesSeleccionadasSelectList = new HashSet<AlbumCancionesSeleccionadas>();

            TICaseFormat = new CultureInfo("es-MX", false).TextInfo;

            //cuando el álbum no contiene tracklist, inicializamos la lista de canciones para evitar errores
            if (listaCancionesSeleccionadas == null)
            {
                listaCancionesSeleccionadas = new List<string>();
            }

            try
            {
                /*debido a que listaCancionesSeleccionadas es una lista de valores "id-posicion", creamos una lista 
                 * que contenga las canciones seleccionadas */
                listaCancionesSeleccionadas.ForEach(t =>
                {
                    string[] arrData = t.Split('-');    //cada item contiene el valor "id-posición"
                    //con el primer item (id) buscamos la canción
                    Cancion cancion = db.Canciones.Find(Convert.ToInt32(arrData[0]));
                    cancionesSeleccionadas.Add(cancion);

                    /*creamos una lista compatible con SelectList y la llenamos con los valores necesarios.
                     * Esta lista sólo la usaremos para asignarla a ViewBag en el caso de que el ModelState sea rechazado */
                    cancionesSeleccionadasSelectList.Add(new AlbumCancionesSeleccionadas()
                    {
                        CancionIDPosicion = t,
                        Descripcion = string.Format(formatoArtistaCancion, cancion.Artistas.FirstOrDefault().Nombre, cancion.Nombre)
                    });
                });

                //el álbum debe tener al menos un artista asignado
                if (listaArtistasSeleccionados == null)
                {
                    ModelState.AddModelError("", "Asigna al álbum por lo menos un artista.");
                }
                else
                {
                    //asignamos los artistas a la lista de artistas obteniéndolos de la lista de ids de artistas seleccionados
                    listaArtistasSeleccionados.ForEach(a => artistasSeleccionados.Add(db.Artistas.Find(a)));

                    if (album.Formato_FormatoID > 0)
                    {
                        /*el modelo requiere que se asigne un formato y al no recibirlo de la vista
                         * genera un error de modelo. Como sí tenemos el id del formato, buscamos el formato
                         * y lo asignamos al álbum y posteriormente eliminamos el error de modelo */
                        album.Formato = db.Formatos.Find(album.Formato_FormatoID);
                        ModelState["Formato"].Errors.Clear();
                        UpdateModel(album);
                    }

                    if (ModelState.IsValid)
                    {
                        //validación de album duplicado en la BD
                        if (!db.Albums.AsEnumerable().Any(s =>
                            s.Nombre.ToLower() == album.Nombre.Trim().ToLower()
                                && s.Artistas.Count == artistasSeleccionados.Count
                                && !s.Artistas.Except(artistasSeleccionados, new Models.ComparadorArtista())
                                    .Any()
                                && s.Formato.FormatoID == album.Formato_FormatoID
                                && s.AlbumID != album.AlbumID))
                        {
                            //buscamos el album de la BD porque el que usa MVC tiene estado dettached y
                            //generaría error al hacer la persistencia.
                            Album albumDB = db.Albums.Find(album.AlbumID);

                            //copiamos los datos de la copia modificada al original de la BD.
                            albumDB.Nombre = TICaseFormat.ToTitleCase(album.Nombre.Trim());
                            albumDB.Formato_FormatoID = album.Formato_FormatoID;
                            albumDB.Formato = album.Formato;
                            albumDB.FechaAdquisicion = album.FechaAdquisicion;
                            albumDB.FechaGrabacion = album.FechaGrabacion;
                            albumDB.DiscogsReleaseCode = album.DiscogsReleaseCode;

                            //a partir de aquí, nos olvidamos del objeto album y trabajamos con albumDB.
                            //obtenemos la lista de artistas a eliminar (los artistas desasignados por el usuario)
                            var artistasAEliminar = albumDB.Artistas.AsEnumerable()
                                .Except(artistasSeleccionados, new Models.ComparadorArtista());

                            artistasAEliminar.ToList().ForEach(s => albumDB.Artistas.Remove(s));

                            //obtenemos la lista de artistas a agregar (los artistas asignados por el usuario)
                            var artistasAgregados = artistasSeleccionados.Except(albumDB.Artistas.AsEnumerable(),
                                new Models.ComparadorArtista());

                            artistasAgregados.ToList().ForEach(s =>
                            {
                                albumDB.Artistas.Add(s);
                                db.Entry(s).State = System.Data.Entity.EntityState.Unchanged;
                            });

                            //creamos una estructura de datos con el tracklist final
                            ICollection<CancionAlbums> tracklistActual = new HashSet<CancionAlbums>();
                            listaCancionesSeleccionadas
                                .ForEach(t =>
                                {
                                    CancionAlbums track = new CancionAlbums();
                                    string[] arrData = t.Split('-');
                                    track.Cancion = db.Canciones.Find(Convert.ToInt32(arrData[0]));
                                    track.Cancion_CancionID = track.Cancion.CancionID;
                                    track.Album = albumDB;
                                    track.Album_AlbumID = track.Album.AlbumID;
                                    track.Posicion = Convert.ToInt16(arrData[1]);
                                    tracklistActual.Add(track);
                                });

                            //obtenemos la lista de canciones a eliminar (las canciones desasignadas por el usuario)
                            var cancionesAEliminar = albumDB.Tracklist.AsEnumerable()
                                .Except(tracklistActual, new Models.ComparadorCancionAlbums());

                            cancionesAEliminar.ToList().ForEach(s => albumDB.Tracklist.Remove(s));

                            //Actualización del orden de los tracks existentes (las canciones que permanecen)
                            var cancionesExistentes = tracklistActual.Intersect(albumDB.Tracklist.AsEnumerable(),
                                new Models.ComparadorCancionAlbums());
                            cancionesExistentes.ToList().ForEach(s => db.CancionAlbums.Find(s.Cancion_CancionID, s.Album_AlbumID)
                                .Posicion = s.Posicion);

                            //obtenemos la lista de canciones a agregar (las canciones asignadas por el usuario)
                            var cancionesAgregadas = tracklistActual.Except(albumDB.Tracklist.AsEnumerable(),
                                new Models.ComparadorCancionAlbums());
                            cancionesAgregadas.ToList().ForEach(s => albumDB.Tracklist.Add(s));

                            //db.Entry(albumDB).State = EntityState.Modified;
                            db.SaveChanges();

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "El álbum ya está registrado en la BD.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al agregar el álbum. " + e.Message);
            }

            //volvemos a llenar las listas de artistas y canciones para regresar la info. al usuario
            try
            {
                ViewBag.Formato_FormatoID = new SelectList(db.Formatos.OrderBy(f => f.Descripcion), "FormatoID", "Descripcion", album.Formato_FormatoID);
                ViewBag.listaArtistas = new SelectList(db.Artistas.AsEnumerable().Except(artistasSeleccionados).OrderBy(a => a.Nombre), "ArtistaID", "Nombre");
                ViewBag.listaArtistasSeleccionados = new SelectList(artistasSeleccionados, "ArtistaID", "Nombre");
                ViewBag.listaCanciones = new SelectList(
                    db.Canciones.AsEnumerable().Except(cancionesSeleccionadas).OrderBy(c => c.Artistas.FirstOrDefault().Nombre).ThenBy(c => c.Nombre)
                    .Select(c => new
                    {
                        CancionID = c.CancionID,
                        Descripcion = string.Format(formatoArtistaCancion, c.Artistas.FirstOrDefault().Nombre, c.Nombre)
                    }), "CancionID", "Descripcion");
                ViewBag.listaCancionesSeleccionadas = new SelectList(cancionesSeleccionadasSelectList, "CancionIDPosicion", "Descripcion");
            }
            catch (Exception)
            {
                if (ViewBag.Formato_FormatoID == null)
                {
                    ViewBag.Formato_FormatoID = new SelectList(new List<SelectList>(), "FormatoID", "Descripcion");
                }
                if (ViewBag.listaArtistas == null)
                {
                    ViewBag.listaArtistas = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
                }
                if (ViewBag.listaArtistasSeleccionados == null)
                {
                    ViewBag.listaArtistasSeleccionados = new SelectList(new List<SelectList>(), "ArtistaID", "Nombre");
                }
                if (ViewBag.listaCanciones == null)
                {
                    ViewBag.listaCanciones = new SelectList(new List<SelectList>(), "CancionID", "Descripcion");
                }
                if (ViewBag.listaCancionesSeleccionadas == null)
                {
                    ViewBag.listaCancionesSeleccionadas = new SelectList(new List<SelectList>(), "CancionIDPosicion", "Descripcion");
                }
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            AlbumViewVM albumVM = new AlbumViewVM(album);
            return View(albumVM);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
