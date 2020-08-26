using adminlte.AdminCatalogoMensualidadService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AdminCatalogoMensualidadController : BaseController
    {
        // GET: AdminCatalogoMensualidad
        public ActionResult AdminCatalogoMensualidadLista()
        {
            AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
            List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminCatalogoMensualidad);
        }

        [HttpGet]
        public ActionResult AdminCatalogoMensualidadNuevo(string MensajeError = "")
        {
            AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
            AdminCatalogoMensualidadSet setAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminCatalogoMensualidad);
        }

        [HttpPost]
        public ActionResult AdminCatalogoMensualidadNuevo(AdminCatalogoMensualidadSet setAdminCatalogoMensualidad)
        {
            string Mensualidad = string.Empty;
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();

            if (setAdminCatalogoMensualidad.ltAdminCatalogoMensualidad != null)
            {
                AdminCatalogoMensualidadEntity etAdminCatalogoMensualidad = setAdminCatalogoMensualidad.ltAdminCatalogoMensualidad.First();
                if (etAdminCatalogoMensualidad.Mensualidad != "")
                {
                    AdminCatalogoMensualidadSet setAdminCatalogoMensualidadNuevo = AdminCatalogoMensualidad.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    AdminCatalogoMensualidadEntity etAdminCatalogoMensualidadNuevo = setAdminCatalogoMensualidadNuevo.ltAdminCatalogoMensualidad.First();

                    etAdminCatalogoMensualidadNuevo.Mensualidad = etAdminCatalogoMensualidad.Mensualidad;
                    etAdminCatalogoMensualidadNuevo.Descripcion = etAdminCatalogoMensualidad.Descripcion == null ? "" : etAdminCatalogoMensualidad.Descripcion;
                    etAdminCatalogoMensualidadNuevo.UEstado = AdminCatalogoMensualidadService.Estado.Added;
                    Mensualidad = etAdminCatalogoMensualidad.Mensualidad;

                    NumError = AdminCatalogoMensualidad.WebGuardar(setAdminCatalogoMensualidadNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    
                    if (NumError != 0)
                    {
                        MensajeError = "Error al  guardar los datos en el servidor.";
                    }
                }
                else
                {
                    NumError = 1;
                    MensajeError = "La mensualidad es un campo obligatorio.";
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Hubo un error al mandar a guardar los datos.";
            }

            if (NumError == 0)
            {
                //return RedirectToAction("AdminCatalogoMensualidadEditar", new { Mensualidad = Mensualidad });
                return RedirectToAction("AdminCatalogoMensualidadLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setAdminCatalogoMensualidad);
            }

        }

        //[HttpGet]
        //public ActionResult AdminCatalogoMensualidadEditar(string Mensualidad, string MensajeError = "")
        //{
        //    AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
        //    AdminCatalogoMensualidadSet setAdminCatalogoMensualidad = new AdminCatalogoMensualidadSet();

        //    setAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //    ViewBag.MensajeError = MensajeError;

        //    return View(setAdminCatalogoMensualidad);
        //}
        //[HttpPost]
        //public ActionResult AdminCatalogoMensualidadEditar(string Mensualidad, AdminCatalogoMensualidadSet setAdminCatalogoMensualidadForm)
        //{
        //    long NumError = 0;
        //    string MensajeError = string.Empty;
        //    AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
        //    AdminCatalogoMensualidadEntity etAdminCatalogoMensualidadForm = setAdminCatalogoMensualidadForm.ltAdminCatalogoMensualidad.First();
        //    if (etAdminCatalogoMensualidadForm != null)
        //    {
        //        AdminCatalogoMensualidadSet setAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //        AdminCatalogoMensualidadEntity etAdminCatalogoMensualidad = setAdminCatalogoMensualidad.ltAdminCatalogoMensualidad.First();

        //        etAdminCatalogoMensualidad.Observacion = etAdminCatalogoMensualidadForm.Observacion == null ? "" : etAdminCatalogoMensualidadForm.Observacion;
        //        etAdminCatalogoMensualidad.UEstado = AdminCatalogoMensualidadService.Estado.Modified;

        //        NumError = AdminCatalogoMensualidad.WebGuardar(setAdminCatalogoMensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //        if (NumError != 0)
        //        {
        //            MensajeError = "Hubo un error al guardar en el servidor";
        //        }
        //    }
        //    else
        //    {
        //        NumError = 1;
        //        MensajeError = "El formulario contiene un error.";
        //    }            

        //    if (NumError == 0)
        //    {
        //        return RedirectToAction("ASACuestionarioListaPregunta");
        //    }
        //    else
        //    {
        //        return RedirectToAction("AdminCatalogoMensualidadEditar", new { Mensualidad = Mensualidad, MensajeError = MensajeError });
        //    }
        //}
        //public ActionResult AdminCatalogoMensualidadEliminar(string Mensualidad, string MensajeError = "")
        //{
        //    AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
        //    AdminCatalogoMensualidadSet setAdminCatalogoMensualidad = new AdminCatalogoMensualidadSet();

        //    setAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        
        //    ViewBag.MensajeError = MensajeError;

        //    return View(setAdminCatalogoMensualidad);
        //}
        //[HttpPost, ActionName("AdminCatalogoMensualidadEliminar")]
        //public ActionResult AdminCatalogoMensualidadEliminarConfirmacion(string Mensualidad)
        //{
        //    long NumError = 0;
        //    string MensajeError = string.Empty;
        //    AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();

        //    if (Mensualidad != 0)
        //    {
        //        AdminCatalogoMensualidadSet setAdminCatalogoMensualidadBorrar = AdminCatalogoMensualidad.WebSeleccionar(Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

        //        NumError = AdminCatalogoMensualidad.WebEliminar(setAdminCatalogoMensualidadBorrar, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
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
        //        return RedirectToAction("AdminCatalogoMensualidadEliminar", new { Mensualidad = Mensualidad, MensajeError = MensajeError });
        //    }
        //}
    }
}