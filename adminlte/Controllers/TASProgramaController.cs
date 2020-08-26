using adminlte.TASProgramaService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASProgramaController : BaseController
    {
        // GET: TASPrograma
        public ActionResult TASProgramaLista()
        {
            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            var ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASPrograma);
        }

        [HttpGet]
        public ActionResult TASProgramaNuevo(string MensajeError = "")
        {
            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            TASProgramaSet setTASPrograma = TASPrograma.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);            

            ViewBag.MensajeError = MensajeError;

            return View(setTASPrograma);
        }

        [HttpPost]
        public ActionResult TASProgramaNuevo(TASProgramaSet setTASPrograma)
        {
            long NumError = 0;
            string Programa = string.Empty;
            string MensajeError = string.Empty;

            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();

            if (setTASPrograma.ltTASPrograma != null)
            {
                TASProgramaEntity etTASPrograma = setTASPrograma.ltTASPrograma.First();
                if (etTASPrograma.Programa == null || etTASPrograma.Programa == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Programa. ";
                }                

                if (NumError == 0)
                {
                    TASProgramaSet setTASProgramaNuevo = TASPrograma.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASProgramaEntity etTASProgramaNuevo = setTASProgramaNuevo.ltTASPrograma.First();
                    etTASProgramaNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASProgramaNuevo.Programa = etTASPrograma.Programa;
                    etTASProgramaNuevo.Descripcion = etTASPrograma.Descripcion == null ? "" : etTASPrograma.Descripcion;
                    etTASProgramaNuevo.Observacion = etTASPrograma.Observacion == null ? "" : etTASPrograma.Observacion;

                    etTASProgramaNuevo.UEstado = TASProgramaService.Estado.Added;
                    Programa = etTASPrograma.Programa;
                    NumError = TASPrograma.WebGuardar(setTASProgramaNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASProgramaEditar", new { SubCompania = (string)Session["SesionSubCompania"], Programa = Programa });
            }
            else
            {                
                ViewBag.MensajeError = MensajeError;
                return View(setTASPrograma);
            }

        }

        [HttpGet]
        public ActionResult TASProgramaEditar(string SubCompania, string Programa, string MensajeError = "")
        {
            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            TASProgramaSet setTASPrograma = new TASProgramaSet();
            setTASPrograma = TASPrograma.WebSeleccionar(SubCompania, Programa, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);            

            ViewBag.MensajeError = MensajeError;

            return View(setTASPrograma);
        }

        [HttpPost]
        public ActionResult TASProgramaEditar(TASProgramaSet setTASPrograma)
        {
            long NumError = 0;
            string Programa = string.Empty;
            string MensajeError = string.Empty;

            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();

            if (setTASPrograma.ltTASPrograma != null)
            {
                TASProgramaEntity etTASPrograma = setTASPrograma.ltTASPrograma.First();
                if (etTASPrograma.Programa == null || etTASPrograma.Programa == "")
                {
                    NumError = 1;
                    MensajeError += "Debe existir un Programa. ";
                }

                if (NumError == 0)
                {
                    TASProgramaSet setTASProgramaEdit = TASPrograma.WebSeleccionar((string)Session["SesionSubCompania"], etTASPrograma.Programa, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASProgramaEntity etTASProgramaEdit = setTASProgramaEdit.ltTASPrograma.First();
                    //CodigoTAS Same as EstudianteCI
                    etTASProgramaEdit.Descripcion = etTASPrograma.Descripcion == null ? "" : etTASPrograma.Descripcion;                    
                    etTASProgramaEdit.Observacion = etTASPrograma.Observacion == null ? "" : etTASPrograma.Observacion;

                    etTASProgramaEdit.UEstado = TASProgramaService.Estado.Modified;
                    Programa = etTASPrograma.Programa;
                    NumError = TASPrograma.WebGuardar(setTASProgramaEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASProgramaLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setTASPrograma);
            }
        }
    }
}