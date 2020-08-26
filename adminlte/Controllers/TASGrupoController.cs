using adminlte.TASGrupoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASGrupoController : BaseController
    {
        // GET: TASGrupo
        public ActionResult TASGrupoLista()
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            var ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASGrupo);
        }

        [HttpGet]
        public ActionResult TASGrupoNuevo(string MensajeError = "")
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            TASGrupoSet setTASGrupo = TASGrupo.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setTASGrupo);
        }

        [HttpPost]
        public ActionResult TASGrupoNuevo(TASGrupoSet setTASGrupo)
        {
            long NumError = 0;
            string Grupo = string.Empty;
            string MensajeError = string.Empty;

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();

            if (setTASGrupo.ltTASGrupo != null)
            {
                TASGrupoEntity etTASGrupo = setTASGrupo.ltTASGrupo.First();
                if (etTASGrupo.Grupo == null || etTASGrupo.Grupo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Grupo. ";
                }

                if (NumError == 0)
                {
                    TASGrupoSet setTASGrupoNuevo = TASGrupo.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASGrupoEntity etTASGrupoNuevo = setTASGrupoNuevo.ltTASGrupo.First();
                    etTASGrupoNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASGrupoNuevo.Grupo = etTASGrupo.Grupo;
                    etTASGrupoNuevo.Descripcion = etTASGrupo.Descripcion == null ? "" : etTASGrupo.Descripcion;
                    etTASGrupoNuevo.Observacion = etTASGrupo.Observacion == null ? "" : etTASGrupo.Observacion;

                    etTASGrupoNuevo.UEstado = TASGrupoService.Estado.Added;
                    Grupo = etTASGrupo.Grupo;
                    NumError = TASGrupo.WebGuardar(setTASGrupoNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASGrupoEditar", new { SubCompania = (string)Session["SesionSubCompania"], Grupo = Grupo });
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASGrupo);
            }

        }

        [HttpGet]
        public ActionResult TASGrupoEditar(string SubCompania, string Grupo, string MensajeError = "")
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            TASGrupoSet setTASGrupo = new TASGrupoSet();
            setTASGrupo = TASGrupo.WebSeleccionar(SubCompania, Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setTASGrupo);
        }

        [HttpPost]
        public ActionResult TASGrupoEditar(TASGrupoSet setTASGrupo)
        {
            long NumError = 0;
            string Grupo = string.Empty;
            string MensajeError = string.Empty;

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();

            if (setTASGrupo.ltTASGrupo != null)
            {
                TASGrupoEntity etTASGrupo = setTASGrupo.ltTASGrupo.First();
                if (etTASGrupo.Grupo == null || etTASGrupo.Grupo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe existir un Grupo. ";
                }

                if (NumError == 0)
                {
                    TASGrupoSet setTASGrupoEdit = TASGrupo.WebSeleccionar((string)Session["SesionSubCompania"], etTASGrupo.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASGrupoEntity etTASGrupoEdit = setTASGrupoEdit.ltTASGrupo.First();
                    //CodigoTAS Same as EstudianteCI
                    etTASGrupoEdit.Descripcion = etTASGrupo.Descripcion == null ? "" : etTASGrupo.Descripcion;
                    etTASGrupoEdit.Observacion = etTASGrupo.Observacion == null ? "" : etTASGrupo.Observacion;

                    etTASGrupoEdit.UEstado = TASGrupoService.Estado.Modified;
                    Grupo = etTASGrupo.Grupo;
                    NumError = TASGrupo.WebGuardar(setTASGrupoEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASGrupoLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASGrupo);
            }
        }
    }
}