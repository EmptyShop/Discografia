using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVCDiscografia.Models;
using System.Globalization;

namespace MVCDiscografia.Controllers
{
    [Authorize]
    public class ArtistasController : Controller
    {
        private DiscografiaDB db = new DiscografiaDB();
        private TextInfo TICaseFormat;

        // GET: Artistas
        public ActionResult Index()
        {
            return View(db.Artistas.OrderBy(a => a.Nombre).ToList());
        }

        // GET: Artistas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artista artista = db.Artistas.Find(id);
            if (artista == null)
            {
                return HttpNotFound();
            }
            return View(artista);
        }

        // GET: Artistas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Artistas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArtistaID,Nombre,Pais")] Artista artista)
        {
            TICaseFormat = new CultureInfo("es-MX", false).TextInfo;
            if (ModelState.IsValid)
            {
                try
                {
                    //validación de artista duplicado
                    if (!db.Artistas.Any(s => s.Nombre.ToLower() == artista.Nombre.Trim().ToLower()))
                    {
                        artista.Nombre = TICaseFormat.ToTitleCase(artista.Nombre.Trim());
                        artista.Pais = artista.Pais != null ? TICaseFormat.ToTitleCase(artista.Pais.Trim()) : artista.Pais;
                        db.Artistas.Add(artista);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "El artista ya existe en la base de datos.");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Ocurrió un error al agregar el artista. " + e.Message);
                }
            }

            return View(artista);
        }

        // GET: Artistas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artista artista = db.Artistas.Find(id);
            if (artista == null)
            {
                return HttpNotFound();
            }
            return View(artista);
        }

        // POST: Artistas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArtistaID,Nombre,Pais")] Artista artista)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //validación de artista duplicado
                    if (!db.Artistas.Any(a => a.Nombre.ToLower() == artista.Nombre.Trim().ToLower()
                        && a.ArtistaID != artista.ArtistaID))
                    {
                        artista.Nombre = artista.Nombre.Trim();
                        artista.Pais = artista.Pais != null ? artista.Pais.Trim() : artista.Pais;

                        db.Entry(artista).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "El artista ya existe en la base de datos.");
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar el artista. " + e.Message);
            }

            return View(artista);
        }

        // GET: Artistas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artista artista = db.Artistas.Find(id);
            if (artista == null)
            {
                return HttpNotFound();
            }
            return View(artista);
        }

        // POST: Artistas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artista artista = db.Artistas.Find(id);
            db.Artistas.Remove(artista);
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
