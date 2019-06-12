using adminlte.AXFUsuarioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AXFUsuarioController : BaseController
    {
        // GET: AXFUsuario
        public ActionResult AXFUsuarioEditar()
        {
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFUsuarioSet setAXFUsuario = AXFUsuario.WebSeleccionar((string)Session["Usuario"],(string)Session["SesionSubCompania"],(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            AXFUsuarioEntity etAXFUsuario = setAXFUsuario.ltAXFUsuario.First();
            etAXFUsuario.Clave = AXFUsuario.WebDesencriptarClave(etAXFUsuario.Clave, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(setAXFUsuario);
        }

        [HttpPost]
        public ActionResult AXFUsuarioEditar(AXFUsuarioSet setAXFUsuarioForm)
        {
            long NumError = 0;
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFUsuarioEntity etAXFUsuarioForm = setAXFUsuarioForm.ltAXFUsuario.First();
            if (etAXFUsuarioForm != null)
            {
                AXFUsuarioSet setAXFUsuario = AXFUsuario.WebSeleccionar((string)Session["Usuario"], (string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AXFUsuarioEntity etAXFUsuario = setAXFUsuario.ltAXFUsuario.First();
                etAXFUsuario.Nombre = etAXFUsuarioForm.Nombre;
                etAXFUsuario.CorreoElectronico = etAXFUsuarioForm.CorreoElectronico;
                etAXFUsuario.Telefono1 = etAXFUsuarioForm.Telefono1;
                //etAXFUsuario.Clave = AXFUsuario.WebEncriptarClave( etAXFUsuarioForm.Clave, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAXFUsuario.Clave = etAXFUsuarioForm.Clave;
                etAXFUsuario.UEstado = AXFUsuarioService.Estado.Modified;

                NumError = AXFUsuario.WebGuardar(setAXFUsuario, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }
            else
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Index");
            }
        }
    }
}