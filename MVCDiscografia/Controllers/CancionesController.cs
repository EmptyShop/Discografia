using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVCDiscografia.Models;
using System.Data.Entity.Infrastructure;
using MVCDiscografia.ViewModels;
using System.Globalization;

namespace MVCDiscografia.Controllers
{
    [Authorize]
    public class CancionesController : Controller
    {
        private DiscografiaDB db = new DiscografiaDB();
        private TextInfo TICaseFormat;

        // GET: Canciones
        public ActionResult Index()
        {
            ICollection<CancionViewVM> lasCanciones = new HashSet<CancionViewVM>();

            //la lista de canciones con artistas incluídos
            db.Canciones.ToList().ForEach(c => lasCanciones.Add(new CancionViewVM(c)));

            return View(lasCanciones.OrderBy(c => c.losArtistas).ThenBy(c => c.Cancion.Nombre));
        }

        // GET: Canciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cancion cancion = db.Canciones.Find(id);
            if (cancion == null)
            {
                return HttpNotFound();
            }
            //detalle de la canción con artistas incluídos
            CancionViewVM cancionView = new CancionViewVM(cancion);
            return View(cancionView);
        }

        // GET: Canciones/Create
        public ActionResult Create()
        {
            CancionCreateEditVM cancion = new CancionCreateEditVM();

            // una nueva canción con la lista de artistas para seleccionar
            LlenaListaArtistas(cancion);
            return View(cancion);
        }

        // POST: Canciones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cancion,ArtistasSeleccionados")] CancionCreateEditVM cancionVM)
        {
            TICaseFormat = new CultureInfo("es-MX", false).TextInfo;
            try
            {
                if (ModelState.IsValid)
                {
                    //la canción debe contener al menos un artista
                    if (cancionVM.ArtistasSeleccionados.Count == 0)
                    {
                        ModelState.AddModelError("", "Selecciona el artista de la canción.");
                    }
                    if (ModelState.IsValid)
                    {
                        //asignamos los artistas a la canción obteniéndolos de la lista de ids de artistas seleccionados
                        cancionVM.ArtistasSeleccionados.ToList().ForEach(a => cancionVM.Cancion.Artistas.Add(
                            db.Artistas.Find(a)
                        ));

                        //validación de canción duplicada en la BD
                        if (!db.Canciones.AsEnumerable().Any(s => s.Nombre.ToLower() == cancionVM.Cancion.Nombre.Trim().ToLower()
                            && s.Artistas.Count == cancionVM.ArtistasSeleccionados.Count
                            && !s.Artistas.Except(cancionVM.Cancion.Artistas, new Models.ComparadorArtista())
                            .Any()))
                        {
                            cancionVM.Cancion.Nombre = TICaseFormat.ToTitleCase(cancionVM.Cancion.Nombre.Trim());

                            db.Canciones.Add(cancionVM.Cancion);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "La canción ya está registrada en la base de datos.");
                        }
                    }
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "No se agregó el registro. Demasiados intentos.");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al agregar la canción: \n" + e.Message);
            }

            LlenaListaArtistas(cancionVM);
            return View(cancionVM);
        }

        // GET: Canciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cancion cancion = db.Canciones.Find(id);
            if (cancion == null)
            {
                return HttpNotFound();
            }
            CancionCreateEditVM cancionEdit = new CancionCreateEditVM();

            //la canción con la lista de artistas para agregar o quitar
            cancionEdit.Cancion = cancion;
            LlenaListaArtistas(cancionEdit);

            return View(cancionEdit);
        }

        // POST: Canciones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Cancion,ArtistasSeleccionados")] CancionCreateEditVM cancionVM)
        {
            TICaseFormat = new CultureInfo("es-MX", false).TextInfo;
            try
            {
                if (ModelState.IsValid)
                {
                    //la canción debe contener al menos un artista
                    if (cancionVM.ArtistasSeleccionados.Count == 0)
                    {
                        ModelState.AddModelError("", "Selecciona el artista de la canción.");
                    }
                    if (ModelState.IsValid)
                    {
                        //validación de canción duplicada en la BD
                        if (!db.Canciones.AsEnumerable().Any(s => s.Nombre.ToLower() == cancionVM.Cancion.Nombre.Trim().ToLower()
                            && s.Artistas.Count == cancionVM.ArtistasSeleccionados.Count
                            && !s.Artistas.Except(cancionVM.Cancion.Artistas, new Models.ComparadorArtista())
                            .Any()
                            && s.CancionID != cancionVM.Cancion.CancionID))
                        {
                            //buscamos la canción original en la BD
                            Cancion cancion = db.Canciones.Find(cancionVM.Cancion.CancionID);

                            //aplicamos los cambios recibidos
                            cancion.Nombre = TICaseFormat.ToTitleCase(cancionVM.Cancion.Nombre.Trim());
                            cancion.Duracion = cancionVM.Cancion.Duracion;
                            cancion.Año = cancionVM.Cancion.Año;

                            //generamos la lista de artistas seleccionados por el usuario
                            var artistasSeleccionados = db.Artistas.Where(a => 
                                cancionVM.ArtistasSeleccionados.Contains(a.ArtistaID)).ToList();

                            //obtenemos la lista de artistas a eliminar (los artistas desasignados por el usuario)
                            var artistasAEliminar = cancion.Artistas.AsEnumerable()
                                .Except(artistasSeleccionados, new Models.ComparadorArtista());

                            artistasAEliminar.ToList().ForEach(s => cancion.Artistas.Remove(s));

                            //obtenemos la lista de artistas a agregar (los artistas asignados por el usuario)
                            var artistasAgregados = artistasSeleccionados.Except(cancion.Artistas.AsEnumerable(), 
                                new Models.ComparadorArtista());

                            artistasAgregados.ToList().ForEach(s =>
                            {
                                cancion.Artistas.Add(s);
                                db.Entry(s).State = System.Data.Entity.EntityState.Unchanged;
                            });

                            db.Entry(cancion).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "La canción ya está registrada en la base de datos.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al modificar la canción: \n" + e.Message);
            }
            LlenaListaArtistas(cancionVM);
            return View(cancionVM);
        }

        // GET: Canciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cancion cancion = db.Canciones.Find(id);
            if (cancion == null)
            {
                return HttpNotFound();
            }
            //la canción con artistas
            CancionViewVM cancionView = new CancionViewVM(cancion);
            return View(cancionView);
        }

        // POST: Canciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cancion cancion = db.Canciones.Find(id);
            db.Canciones.Remove(cancion);
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

        /*Llena las listas de artistas no asignados a la canción (en ListaArtistas)
         * y artistas asignados a la canción (en ListaArtistasCancion)*/
        private void LlenaListaArtistas(CancionCreateEditVM cancionVM){
            List<Artista> listaArtistas, artistasCancion;

            //canción nueva: todos los artistas en no asignados, ningún artista en asignados
            if (cancionVM.Cancion == null || cancionVM.Cancion.Artistas.Count == 0)
            {
                listaArtistas = db.Artistas.OrderBy(a => a.Nombre).ToList();
                artistasCancion = new List<Artista>();
            }
            else
            {
                //canción existente: todos los artistas en la lista de no asignados, excepto los asignados;
                //los artistas de la canción en la lista de asignados
                listaArtistas = db.Artistas.AsEnumerable().Except(cancionVM.Cancion.Artistas).ToList();
                artistasCancion = cancionVM.Cancion.Artistas.ToList();
            }

            //creación de las listas compatibles con el campo select de HTML
            cancionVM.ListaArtistas = listaArtistas.Select(a => new SelectListItem
            {
                Text = a.Nombre,
                Value = a.ArtistaID.ToString()
            });

            cancionVM.ListaArtistasCancion = artistasCancion.Select(a => new SelectListItem
            {
                Text = a.Nombre,
                Value = a.ArtistaID.ToString()
            });
        }
    }
}
