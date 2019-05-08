using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVCDiscografia.Models;
using System.Globalization;

namespace MVCDiscografia.Controllers
{
    [Authorize]
    public class FormatosController : Controller
    {
        private DiscografiaDB db = new DiscografiaDB();
        private TextInfo TICaseFormat;

        // GET: Formatos
        public ActionResult Index()
        {
            return View(db.Formatos.ToList());
        }

        // GET: Formatos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formato formato = db.Formatos.Find(id);
            if (formato == null)
            {
                return HttpNotFound();
            }
            return View(formato);
        }

        // GET: Formatos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Formatos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FormatoID,Descripcion")] Formato formato)
        {
            TICaseFormat = new CultureInfo("es-MX", false).TextInfo;
            if (ModelState.IsValid)
            {
                formato.Descripcion = TICaseFormat.ToTitleCase(formato.Descripcion.Trim());
                db.Formatos.Add(formato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formato);
        }

        // GET: Formatos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formato formato = db.Formatos.Find(id);
            if (formato == null)
            {
                return HttpNotFound();
            }
            return View(formato);
        }

        // POST: Formatos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FormatoID,Descripcion")] Formato formato)
        {
            if (ModelState.IsValid)
            {
                formato.Descripcion = formato.Descripcion.Trim();

                db.Entry(formato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(formato);
        }

        // GET: Formatos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formato formato = db.Formatos.Find(id);
            if (formato == null)
            {
                return HttpNotFound();
            }
            return View(formato);
        }

        // POST: Formatos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Formato formato = db.Formatos.Find(id);
            db.Formatos.Remove(formato);
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
