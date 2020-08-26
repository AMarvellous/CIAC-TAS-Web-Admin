using adminlte.TASMateriaService;
using adminlte.TASModuloService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASMateriaController : BaseController
    {
        // GET: TASMateria
        public ActionResult TASMateriaLista()
        {
            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            var ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASMateria);
        }

        [HttpGet]
        public ActionResult TASMateriaNuevo(string MensajeError = "")
        {
            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            TASMateriaSet setTASMateria = TASMateria.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
            var ltTASModulo = TASModulo.WebTASModuloSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASModulo = ltTASModulo.Select(s => s.Modulo);
            ViewBag.MensajeError = MensajeError;

            return View(setTASMateria);
        }

        [HttpPost]
        public ActionResult TASMateriaNuevo(TASMateriaSet setTASMateria)
        {
            long NumError = 0;
            string Materia = string.Empty;
            string MensajeError = string.Empty;

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();

            if (setTASMateria.ltTASMateria != null)
            {
                TASMateriaEntity etTASMateria = setTASMateria.ltTASMateria.First();
                if (etTASMateria.Materia == null || etTASMateria.Materia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Materia. ";
                }

                if (etTASMateria.Modulo == null || etTASMateria.Modulo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Modulo. ";
                }

                if (NumError == 0)
                {
                    TASMateriaSet setTASMateriaNuevo = TASMateria.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASMateriaEntity etTASMateriaNuevo = setTASMateriaNuevo.ltTASMateria.First();
                    etTASMateriaNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASMateriaNuevo.Materia = etTASMateria.Materia;
                    etTASMateriaNuevo.Modulo = etTASMateria.Modulo;
                    etTASMateriaNuevo.Descripcion = etTASMateria.Descripcion == null ? "" : etTASMateria.Descripcion;
                    etTASMateriaNuevo.Observacion = etTASMateria.Observacion == null ? "" : etTASMateria.Observacion;

                    etTASMateriaNuevo.UEstado = TASMateriaService.Estado.Added;
                    Materia = etTASMateria.Materia;
                    NumError = TASMateria.WebGuardar(setTASMateriaNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASMateriaEditar", new { SubCompania = (string)Session["SesionSubCompania"], Materia = Materia });
            }
            else
            {
                TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
                var ltTASModulo = TASModulo.WebTASModuloSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASModulo = ltTASModulo.Select(s => s.Modulo);
                ViewBag.MensajeError = MensajeError;
                return View(setTASMateria);
            }

        }

        [HttpGet]
        public ActionResult TASMateriaEditar(string SubCompania, string Materia, string MensajeError = "")
        {
            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            TASMateriaSet setTASMateria = new TASMateriaSet();
            setTASMateria = TASMateria.WebSeleccionar(SubCompania, Materia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
            var ltTASModulo = TASModulo.WebTASModuloSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASModulo = ltTASModulo.Select(s => s.Modulo);

            ViewBag.MensajeError = MensajeError;

            return View(setTASMateria);
        }

        [HttpPost]
        public ActionResult TASMateriaEditar(TASMateriaSet setTASMateria)
        {
            long NumError = 0;
            string Materia = string.Empty;
            string MensajeError = string.Empty;

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();

            if (setTASMateria.ltTASMateria != null)
            {
                TASMateriaEntity etTASMateria = setTASMateria.ltTASMateria.First();
                if (etTASMateria.Materia == null || etTASMateria.Materia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe existir un Materia. ";
                }

                if (etTASMateria.Modulo == null || etTASMateria.Modulo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un Modulo. ";
                }

                if (NumError == 0)
                {
                    TASMateriaSet setTASMateriaEdit = TASMateria.WebSeleccionar((string)Session["SesionSubCompania"], etTASMateria.Materia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASMateriaEntity etTASMateriaEdit = setTASMateriaEdit.ltTASMateria.First();
                    etTASMateriaEdit.Modulo = etTASMateria.Modulo;
                    etTASMateriaEdit.Descripcion = etTASMateria.Descripcion == null ? "" : etTASMateria.Descripcion;
                    etTASMateriaEdit.Observacion = etTASMateria.Observacion == null ? "" : etTASMateria.Observacion;

                    etTASMateriaEdit.UEstado = TASMateriaService.Estado.Modified;
                    Materia = etTASMateria.Materia;
                    NumError = TASMateria.WebGuardar(setTASMateriaEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASMateriaLista");
            }
            else
            {
                TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
                var ltTASModulo = TASModulo.WebTASModuloSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASModulo = ltTASModulo.Select(s => s.Modulo);
                ViewBag.MensajeError = MensajeError;
                return View(setTASMateria);
            }
        }
    }
}