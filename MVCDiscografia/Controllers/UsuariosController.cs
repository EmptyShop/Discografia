using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVCDiscografia.Models;

namespace MVCDiscografia.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private DiscografiaDB db = new DiscografiaDB();

        // GET: Usuarios
        public ActionResult Index()
        {
            return View(db.Usuarios.OrderBy(s => s.User).ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UsuarioID,User,Password,PasswordConfirm,Estatus")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //validación de usuario duplicado
                    if (!db.Usuarios.Any(s => s.User.ToLower() == usuario.User.Trim().ToLower()))
                    {
                        usuario.User = usuario.User.Trim();
                        usuario.Password = Encriptacion.MD5Hash(usuario.Password);
                        usuario.Estatus = 1;
                        db.Usuarios.Add(usuario);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "El usuario ya existe en la base de datos.");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Ocurrió un error al agregar el usuario. " + e.Message);
                }
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            //generamos una lista para seleccionar los posibles valores de Estatus
            List<SelectListItem> EstatusItems = new List<SelectListItem>();
            EstatusItems.Add(new SelectListItem
            {
                Text = "Activo",
                Value = "1",
            });
            EstatusItems.Add(new SelectListItem
            {
                Text = "Suspendido",
                Value = "0"
            });
            EstatusItems.Where(e => Convert.ToInt32(e.Value) == usuario.Estatus).First().Selected = true;
            ViewBag.Estatus = EstatusItems;

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsuarioID,User,Password,PasswordConfirm,Estatus")] Usuario usuario,
            string cambiarPassword)
        {
            try
            {
                //si no hay cambio de password eliminamos las validaciones de los campos Password y PasswordConfirm
                if (cambiarPassword == "false")
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("PasswordConfirm");
                }

                if (ModelState.IsValid)
                {
                    //validación de usuario duplicado
                    if (!db.Usuarios.Any(s => s.User.ToLower() == usuario.User.Trim().ToLower()
                        && s.UsuarioID != usuario.UsuarioID))
                    {
                        //el objeto usuario tiene estado detached entonces usamos el objeto usuarioDB
                        //que ya está registrado en la base de datos.
                        Usuario usuarioDB = db.Usuarios.Find(usuario.UsuarioID);

                        usuarioDB.User = usuario.User.Trim();

                        if (cambiarPassword == "true")
                        {
                            usuarioDB.Password = Encriptacion.MD5Hash(usuario.Password);
                        }

                        //la columna PasswordConfirm no está en la BD pero es necesaria para validar
                        //el modelo
                        usuarioDB.PasswordConfirm = usuarioDB.Password;
                        usuarioDB.Estatus = usuario.Estatus;

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "El usuario ya existe en la base de datos.");
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar el usuario. " + e.Message);
            }

            //generamos una lista para seleccionar los posibles valores de Estatus
            List<SelectListItem> EstatusItems = new List<SelectListItem>();
            EstatusItems.Add(new SelectListItem
            {
                Text = "Activo",
                Value = "1",
            });
            EstatusItems.Add(new SelectListItem
            {
                Text = "Suspendido",
                Value = "0"
            });
            EstatusItems.Where(e => Convert.ToInt32(e.Value) == usuario.Estatus).First().Selected = true;
            ViewBag.Estatus = EstatusItems;

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
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
