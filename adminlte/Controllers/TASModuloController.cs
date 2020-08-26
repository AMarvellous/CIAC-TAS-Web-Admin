using adminlte.TASModuloService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASModuloController : BaseController
    {
        // GET: TASModulo
        public ActionResult TASModuloLista()
        {
            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
            var ltTASModulo = TASModulo.WebTASModuloSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASModulo);
        }

        [HttpGet]
        public ActionResult TASModuloNuevo(string MensajeError = "")
        {
            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
            TASModuloSet setTASModulo = TASModulo.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setTASModulo);
        }

        [HttpPost]
        public ActionResult TASModuloNuevo(TASModuloSet setTASModulo)
        {
            long NumError = 0;
            string Modulo = string.Empty;
            string MensajeError = string.Empty;

            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();

            if (setTASModulo.ltTASModulo != null)
            {
                TASModuloEntity etTASModulo = setTASModulo.ltTASModulo.First();
                if (etTASModulo.Modulo == null || etTASModulo.Modulo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Modulo. ";
                }

                if (NumError == 0)
                {
                    TASModuloSet setTASModuloNuevo = TASModulo.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASModuloEntity etTASModuloNuevo = setTASModuloNuevo.ltTASModulo.First();
                    etTASModuloNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASModuloNuevo.Modulo = etTASModulo.Modulo;
                    etTASModuloNuevo.Descripcion = etTASModulo.Descripcion == null ? "" : etTASModulo.Descripcion;
                    etTASModuloNuevo.Observacion = etTASModulo.Observacion == null ? "" : etTASModulo.Observacion;

                    etTASModuloNuevo.UEstado = TASModuloService.Estado.Added;
                    Modulo = etTASModulo.Modulo;
                    NumError = TASModulo.WebGuardar(setTASModuloNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASModuloEditar", new { SubCompania = (string)Session["SesionSubCompania"], Modulo = Modulo });
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASModulo);
            }

        }

        [HttpGet]
        public ActionResult TASModuloEditar(string SubCompania, string Modulo, string MensajeError = "")
        {
            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
            TASModuloSet setTASModulo = new TASModuloSet();
            setTASModulo = TASModulo.WebSeleccionar(SubCompania, Modulo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setTASModulo);
        }

        [HttpPost]
        public ActionResult TASModuloEditar(TASModuloSet setTASModulo)
        {
            long NumError = 0;
            string Modulo = string.Empty;
            string MensajeError = string.Empty;

            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();

            if (setTASModulo.ltTASModulo != null)
            {
                TASModuloEntity etTASModulo = setTASModulo.ltTASModulo.First();
                if (etTASModulo.Modulo == null || etTASModulo.Modulo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe existir un Modulo. ";
                }

                if (NumError == 0)
                {
                    TASModuloSet setTASModuloEdit = TASModulo.WebSeleccionar((string)Session["SesionSubCompania"], etTASModulo.Modulo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASModuloEntity etTASModuloEdit = setTASModuloEdit.ltTASModulo.First();
                    //CodigoTAS Same as EstudianteCI
                    etTASModuloEdit.Descripcion = etTASModulo.Descripcion == null ? "" : etTASModulo.Descripcion;
                    etTASModuloEdit.Observacion = etTASModulo.Observacion == null ? "" : etTASModulo.Observacion;

                    etTASModuloEdit.UEstado = TASModuloService.Estado.Modified;
                    Modulo = etTASModulo.Modulo;
                    NumError = TASModulo.WebGuardar(setTASModuloEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASModuloLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASModulo);
            }
        }
    }
}