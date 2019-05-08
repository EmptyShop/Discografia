using System;
using System.Linq;
using System.Web.Mvc;
using MVCDiscografia.Models;
using System.Web.Security;

namespace MVCDiscografia.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private DiscografiaDB db = new DiscografiaDB();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario usuario)
        {
            string passwordEncriptado = !String.IsNullOrEmpty(usuario.Password) ? Encriptacion.MD5Hash(usuario.Password) : "";

            try
            {
                if (usuario.User != null && usuario.Password != String.Empty
                    && db.Usuarios.Any(u => u.User == usuario.User
                    && u.Password == passwordEncriptado
                    && u.Estatus == 1))
                {
                    FormsAuthentication.SetAuthCookie(usuario.User, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario no validado. Inténtalo de nuevo.");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "No se pudo validar el acceso. " + e.Message);
            }
            
            return View(usuario);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
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
