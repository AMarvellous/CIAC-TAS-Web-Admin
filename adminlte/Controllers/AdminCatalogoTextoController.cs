using adminlte.AdminCatalogoTextoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AdminCatalogoTextoController : BaseController
    {
        // GET: AdminCatalogoTexto
        public ActionResult AdminCatalogoTextoLista()
        {
            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
            List<AdminCatalogoTextoEntity> ltAdminCatalogoTexto = AdminCatalogoTexto.WebAdminCatalogoTextoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminCatalogoTexto);
        }

        [HttpGet]
        public ActionResult AdminCatalogoTextoNuevo(string MensajeError = "")
        {
            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
            AdminCatalogoTextoSet setAdminCatalogoTexto = AdminCatalogoTexto.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminCatalogoTexto);
        }

        [HttpPost]        
        public ActionResult AdminCatalogoTextoNuevo(AdminCatalogoTextoSet setAdminCatalogoTexto)
        {
            string Texto = string.Empty;
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();

            if (setAdminCatalogoTexto.ltAdminCatalogoTexto != null)
            {
                AdminCatalogoTextoEntity etAdminCatalogoTexto = setAdminCatalogoTexto.ltAdminCatalogoTexto.First();
                if (etAdminCatalogoTexto.Texto != "")
                {
                    AdminCatalogoTextoSet setAdminCatalogoTextoNuevo = AdminCatalogoTexto.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    AdminCatalogoTextoEntity etAdminCatalogoTextoNuevo = setAdminCatalogoTextoNuevo.ltAdminCatalogoTexto.First();

                    etAdminCatalogoTextoNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etAdminCatalogoTextoNuevo.Texto = etAdminCatalogoTexto.Texto;
                    etAdminCatalogoTextoNuevo.MontoSugerido = etAdminCatalogoTexto.MontoSugerido;
                    etAdminCatalogoTextoNuevo.Descripcion = etAdminCatalogoTexto.Descripcion == null ? "" : etAdminCatalogoTexto.Descripcion;
                    etAdminCatalogoTextoNuevo.UEstado = AdminCatalogoTextoService.Estado.Added;
                    Texto = etAdminCatalogoTexto.Texto;

                    NumError = AdminCatalogoTexto.WebGuardar(setAdminCatalogoTextoNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                    if (NumError != 0)
                    {
                        MensajeError = "Error al  guardar los datos en el servidor.";
                    }
                }
                else
                {
                    NumError = 1;
                    MensajeError = "Texto es un campo obligatorio.";
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Hubo un error al mandar a guardar los datos.";
            }

            if (NumError == 0)
            {
                //return RedirectToAction("AdminCatalogoTextoEditar", new { Texto = Texto });
                return RedirectToAction("AdminCatalogoTextoLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setAdminCatalogoTexto);
            }

        }

        [HttpGet]
        public ActionResult AdminCatalogoTextoEditar(string Texto, string MensajeError = "")
        {
            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
            AdminCatalogoTextoSet setAdminCatalogoTexto = AdminCatalogoTexto.WebSeleccionar((string)Session["SesionSubCompania"], Texto, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.MensajeError = MensajeError;

            return View(setAdminCatalogoTexto);
        }
        [HttpPost]
        public ActionResult AdminCatalogoTextoEditar(string Texto, AdminCatalogoTextoSet setAdminCatalogoTextoForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
            AdminCatalogoTextoEntity etAdminCatalogoTextoForm = setAdminCatalogoTextoForm.ltAdminCatalogoTexto.First();
            if (etAdminCatalogoTextoForm != null)
            {
                AdminCatalogoTextoSet setAdminCatalogoTexto = AdminCatalogoTexto.WebSeleccionar((string)Session["SesionSubCompania"], Texto, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AdminCatalogoTextoEntity etAdminCatalogoTexto = setAdminCatalogoTexto.ltAdminCatalogoTexto.First();

                etAdminCatalogoTexto.MontoSugerido = etAdminCatalogoTextoForm.MontoSugerido;
                etAdminCatalogoTexto.Observacion = etAdminCatalogoTextoForm.Observacion == null ? "" : etAdminCatalogoTextoForm.Observacion;
                etAdminCatalogoTexto.UEstado = AdminCatalogoTextoService.Estado.Modified;

                NumError = AdminCatalogoTexto.WebGuardar(setAdminCatalogoTexto, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (NumError != 0)
                {
                    MensajeError = "Hubo un error al guardar en el servidor";
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "El formulario contiene un error.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("AdminCatalogoTextoLista");
            }
            else
            {
                return RedirectToAction("AdminCatalogoTextoEditar", new { Texto = Texto, MensajeError = MensajeError });
            }
        }
        //public ActionResult AdminCatalogoTextoEliminar(string Mensualidad, string MensajeError = "")
        //{
        //    AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
        //    AdminCatalogoTextoSet setAdminCatalogoTexto = new AdminCatalogoTextoSet();

        //    setAdminCatalogoTexto = AdminCatalogoTexto.WebSeleccionar((string)Session["SesionSubCompania"], Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

        //    ViewBag.MensajeError = MensajeError;

        //    return View(setAdminCatalogoTexto);
        //}
        //[HttpPost, ActionName("AdminCatalogoTextoEliminar")]
        //public ActionResult AdminCatalogoTextoEliminarConfirmacion(string Mensualidad)
        //{
        //    long NumError = 0;
        //    string MensajeError = string.Empty;
        //    AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();

        //    if (Mensualidad != 0)
        //    {
        //        AdminCatalogoTextoSet setAdminCatalogoTextoBorrar = AdminCatalogoTexto.WebSeleccionar(Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

        //        NumError = AdminCatalogoTexto.WebEliminar(setAdminCatalogoTextoBorrar, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //        if (NumError != 0)
        //        {
        //            MensajeError = "Error al eliminar la pregunta.";
        //        }
        //    }
        //    else
        //    {
        //        NumError = 1;
        //        MensajeError = "Numero de pregunta invalida para borrar.";
        //    }

        //    if (NumError == 0)
        //    {
        //        return RedirectToAction("ASACuestionarioListaPregunta");
        //    }
        //    else
        //    {
        //        return RedirectToAction("AdminCatalogoTextoEliminar", new { Mensualidad = Mensualidad, MensajeError = MensajeError });
        //    }
        //}
    }
}