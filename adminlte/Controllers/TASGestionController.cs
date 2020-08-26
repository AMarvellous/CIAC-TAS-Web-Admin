using adminlte.TASGestionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASGestionController : BaseController
    {
        // GET: TASGestion
        public ActionResult TASGestionLista()
        {
            TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();
            var ltTASGestion = TASGestion.WebTASGestionSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASGestion);
        }

        [HttpGet]
        public ActionResult TASGestionNuevo(string MensajeError = "")
        {
            TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();
            TASGestionSet setTASGestion = TASGestion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setTASGestion);
        }

        [HttpPost]
        public ActionResult TASGestionNuevo(TASGestionSet setTASGestion)
        {
            long NumError = 0;
            string Gestion = string.Empty;
            string MensajeError = string.Empty;

            TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();

            if (setTASGestion.ltTASGestion != null)
            {
                TASGestionEntity etTASGestion = setTASGestion.ltTASGestion.First();
                if (etTASGestion.Gestion == null || etTASGestion.Gestion == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Gestion. ";
                }

                if (NumError == 0)
                {
                    TASGestionSet setTASGestionNuevo = TASGestion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASGestionEntity etTASGestionNuevo = setTASGestionNuevo.ltTASGestion.First();
                    etTASGestionNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASGestionNuevo.Gestion = etTASGestion.Gestion;
                    etTASGestionNuevo.FechaIni = DateTime.Now;
                    etTASGestionNuevo.FechaFin = DateTime.Now;
                    etTASGestionNuevo.Descripcion = etTASGestion.Descripcion == null ? "" : etTASGestion.Descripcion;
                    etTASGestionNuevo.Observacion = etTASGestion.Observacion == null ? "" : etTASGestion.Observacion;

                    etTASGestionNuevo.UEstado = TASGestionService.Estado.Added;
                    Gestion = etTASGestion.Gestion;
                    NumError = TASGestion.WebGuardar(setTASGestionNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASGestionEditar", new { SubCompania = (string)Session["SesionSubCompania"], Gestion = Gestion });
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASGestion);
            }

        }

        [HttpGet]
        public ActionResult TASGestionEditar(string SubCompania, string Gestion, string MensajeError = "")
        {
            TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();
            TASGestionSet setTASGestion = new TASGestionSet();
            setTASGestion = TASGestion.WebSeleccionar(SubCompania, Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setTASGestion);
        }

        [HttpPost]
        public ActionResult TASGestionEditar(TASGestionSet setTASGestion)
        {
            long NumError = 0;
            string Gestion = string.Empty;
            string MensajeError = string.Empty;

            TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();

            if (setTASGestion.ltTASGestion != null)
            {
                TASGestionEntity etTASGestion = setTASGestion.ltTASGestion.First();
                if (etTASGestion.Gestion == null || etTASGestion.Gestion == "")
                {
                    NumError = 1;
                    MensajeError += "Debe existir un Gestion. ";
                }

                if (NumError == 0)
                {
                    TASGestionSet setTASGestionEdit = TASGestion.WebSeleccionar((string)Session["SesionSubCompania"], etTASGestion.Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASGestionEntity etTASGestionEdit = setTASGestionEdit.ltTASGestion.First();
                    //CodigoTAS Same as EstudianteCI
                    etTASGestionEdit.Descripcion = etTASGestion.Descripcion == null ? "" : etTASGestion.Descripcion;
                    etTASGestionEdit.Observacion = etTASGestion.Observacion == null ? "" : etTASGestion.Observacion;

                    etTASGestionEdit.UEstado = TASGestionService.Estado.Modified;
                    Gestion = etTASGestion.Gestion;
                    NumError = TASGestion.WebGuardar(setTASGestionEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASGestionLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASGestion);
            }
        }
    }
}