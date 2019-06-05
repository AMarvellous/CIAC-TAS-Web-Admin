using adminlte.AXFSesionService;
using adminlte.AXFUsuarioService;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            ViewBag.ltMensajes = TempData["ltMensajes"] as List<string>;

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string Usuario, string Password)
        {
            if (!ModelState.IsValid)
            {
                return View(/*model*/);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return View(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //}
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            string SubCompania = "CIACTAS";
            string SesionResp = string.Empty;

            if (Usuario != "" && Password != "" && SubCompania != "")
            {
                //Es login normal
                //Verificamos si tiene sesion
                List<AXFSesionEntity> ltAXFSesion = AXFSesion.WebAXFSesionSeleccionarXUsuarioSubCompania(Usuario, SubCompania);
                if (ltAXFSesion.Count > 0)
                {
                    bool UsuarioValido = AXFUsuario.WebAXFUsuarioEsValido(Usuario, Password, SubCompania, ltAXFSesion.First().Sesion, SubCompania);
                    if (UsuarioValido)
                    {
                        AXFSesion.WebEliminarSesion(ltAXFSesion.First().Sesion, SubCompania);
                    }
                }

                SesionResp = AXFSesion.WebAccesoValido(Usuario, Password, SubCompania, "CIACTAS PRODUCTO");

                if (SesionResp != "")
                {
                    //Seleccionamos los datos primarios del Usuario para mostrar
                    List<AXFUsuarioEntity> ltAXFUsuario = AXFUsuario.WebAXFUsuarioSeleccionar(Usuario,SubCompania, SesionResp,SubCompania);
                    AXFUsuarioEntity etAXFUsuario = ltAXFUsuario.First();

                    Session["Nombre"] = etAXFUsuario.Nombre;
                    Session["Sesion"] = SesionResp;
                    Session["Usuario"] = Usuario;
                    Session["SesionSubCompania"] = SubCompania;

                    return RedirectToAction("Index", "Index");
                }
                else
                {
                    string msgSessionError;
                    List<AXFSesionMensajeEntity> ltAXFSesionError = null;
                    var ltMsgs = new List<string>();

                    ltAXFSesionError = AXFSesion.WebAXFSesionMensajeSeleccionarKeyAXFSesion("Error" + Usuario, SubCompania, "Error" + Usuario, SubCompania);
                    if (ltAXFSesionError.Count > 0)
                    {
                        foreach (AXFSesionMensajeEntity etAXFSesionMensaje in ltAXFSesionError)
                        {
                            msgSessionError = etAXFSesionMensaje.TextoMensaje;
                            ltMsgs.Add(msgSessionError);
                        }
                        AXFSesion.WebLimpiarMensajes("Error" + Usuario, SubCompania);
                        AXFSesion.WebEliminarSesion("Error" + Usuario, SubCompania);
                    }

                    TempData["ltMensajes"] = ltMsgs;


                    return RedirectToAction("Login");
                }

            }
            else
            {
                //No hay  login
                return RedirectToAction("Login");
            }
        }

        public ActionResult LogOut()
        {
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            try
            {
                long resp = AXFSesion.WebEliminarSesion((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                Session.Clear();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Login", "Login");

        }
    }
}