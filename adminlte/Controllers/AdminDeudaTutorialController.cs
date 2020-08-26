using adminlte.AdminDeudaTutorialService;
using adminlte.TASEstudianteService;
using adminlte.TASGrupoService;
using adminlte.TASMateriaService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AdminDeudaTutorialController : BaseController
    {
        // GET: AdminDeudaTutorial
        public ActionResult AdminDeudaTutorialLista()
        {
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
            List<AdminDeudaTutorialEntity> ltAdminDeudaTutorial = AdminDeudaTutorial.WebAdminDeudaTutorialSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminDeudaTutorial);
        }
        ////Al crear estudiante ya genera una Materia inicial
        //public ActionResult AdminDeudaTutorialNuevo()
        //{

        //}        

        [HttpGet]
        public ActionResult AdminDeudaTutorialEditar(string EstudianteCI, string MensajeError = "")
        {
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
            AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminDeudaTutorial);
        }

        [HttpPost]
        public ActionResult AdminDeudaTutorialEditar(string EstudianteCI, AdminDeudaTutorialSet setAdminDeudaTutorialForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
            AdminDeudaTutorialEntity etAdminDeudaTutorialForm = setAdminDeudaTutorialForm.ltAdminDeudaTutorial.First();

            if (etAdminDeudaTutorialForm != null)
            {
                AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AdminDeudaTutorialEntity etAdminDeudaTutorial = setAdminDeudaTutorial.ltAdminDeudaTutorial.First();
                etAdminDeudaTutorial.Observacion = etAdminDeudaTutorialForm.Observacion;
                etAdminDeudaTutorial.FechaDoc = DateTime.Now;
                etAdminDeudaTutorial.UEstado = AdminDeudaTutorialService.Estado.Modified;

                NumError = AdminDeudaTutorial.WebGuardar(setAdminDeudaTutorial, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

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
                return RedirectToAction("AdminDeudaTutorialLista");
            }
            else
            {
                return RedirectToAction("AdminDeudaTutorialEditar", new { EstudianteCI = EstudianteCI, MensajeError = MensajeError });
            }
        }

        [HttpGet]
        public ActionResult AdminDeudaTutorialDetalleNuevo(string EstudianteCI, string MensajeError = "")
        {
            AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalle = new AdminDeudaTutorialDetalleEntity();
            etAdminDeudaTutorialDetalle.SubCompania = (string)Session["SesionSubCompania"];
            etAdminDeudaTutorialDetalle.EstudianteCI = EstudianteCI;
            etAdminDeudaTutorialDetalle.Materia = string.Empty;
            etAdminDeudaTutorialDetalle.FechaDoc = DateTime.Now;
            etAdminDeudaTutorialDetalle.Monto = 0;
            etAdminDeudaTutorialDetalle.NumeroFactura = string.Empty;
            etAdminDeudaTutorialDetalle.FechaPago = DateTime.Now;
            etAdminDeudaTutorialDetalle.Concepto = string.Empty;
            etAdminDeudaTutorialDetalle.Pagado = false;
            etAdminDeudaTutorialDetalle.Observaciones = string.Empty;
            etAdminDeudaTutorialDetalle.UEstado = AdminDeudaTutorialService.Estado.Added;

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            List<TASMateriaEntity> ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia).ToList();

            ViewBag.MensajeError = MensajeError;

            return PartialView(etAdminDeudaTutorialDetalle);
        }

        [HttpPost]
        public ActionResult AdminDeudaTutorialDetalleNuevo(AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalle)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalleOriginal = etAdminDeudaTutorialDetalle;
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();

            if (etAdminDeudaTutorialDetalle.Materia == null || etAdminDeudaTutorialDetalle.Materia == string.Empty)
            {
                NumError = 1;
                MensajeError = "Debe ingresar una Materia valida.";
            }

            if (AdminDeudaTutorial.WebAdminDeudaTutorialDetalleExiste((string)Session["SesionSubCompania"], etAdminDeudaTutorialDetalle.EstudianteCI, etAdminDeudaTutorialDetalle.Materia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]))
            {
                NumError = 1;
                MensajeError += " La Deuda de tutorial que esta tratando de Adicionar ya esta siendo usada.";
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTutorialDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminDeudaTutorialDetalle.SubCompania = (string)Session["SesionSubCompania"];
                etAdminDeudaTutorialDetalle.FechaDoc = DateTime.Now;
                etAdminDeudaTutorialDetalle.Pagado = false;

                etAdminDeudaTutorialDetalle.NumeroFactura = etAdminDeudaTutorialDetalle.NumeroFactura == null ? "" : etAdminDeudaTutorialDetalle.NumeroFactura;
                etAdminDeudaTutorialDetalle.Concepto = etAdminDeudaTutorialDetalle.Concepto == null ? "" : etAdminDeudaTutorialDetalle.Concepto;
                etAdminDeudaTutorialDetalle.Observaciones = etAdminDeudaTutorialDetalle.Observaciones == null ? "" : etAdminDeudaTutorialDetalle.Observaciones;
                etAdminDeudaTutorialDetalle.UEstado = AdminDeudaTutorialService.Estado.Added;
                setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle.Add(etAdminDeudaTutorialDetalle);

                NumError = AdminDeudaTutorial.WebGuardar(setAdminDeudaTutorial, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminDeudaTutorialEditar", "AdminDeudaTutorial", new { EstudianteCI = etAdminDeudaTutorialDetalle.EstudianteCI }) });
            }
            else
            {
                TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
                List<TASMateriaEntity> ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia).ToList();
                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminDeudaTutorialDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTutorialDetalleEliminar(string EstudianteCI, string Materia)
        {
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
            AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalle = new AdminDeudaTutorialDetalleEntity();

            AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminDeudaTutorialDetalle = setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Materia == Materia).First();

            return PartialView(etAdminDeudaTutorialDetalle);
        }

        [HttpPost]
        public ActionResult AdminDeudaTutorialDetalleEliminar(AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalle)
        {
            long NumError = 0;
            AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalleOriginal = etAdminDeudaTutorialDetalle;
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTutorialDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminDeudaTutorialDetalle = setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle.Where(x => x.EstudianteCI == etAdminDeudaTutorialDetalle.EstudianteCI && x.Materia == etAdminDeudaTutorialDetalle.Materia).First();
                etAdminDeudaTutorialDetalle.UEstado = AdminDeudaTutorialService.Estado.Deleted;
                setAdminDeudaTutorial.ltAdminDeudaTutorialDetalleEliminado.Add(etAdminDeudaTutorialDetalle);
                etAdminDeudaTutorialDetalleOriginal = etAdminDeudaTutorialDetalle;

                NumError = AdminDeudaTutorial.WebGuardar(setAdminDeudaTutorial, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminDeudaTutorialEditar", "AdminDeudaTutorial", new { EstudianteCI = etAdminDeudaTutorialDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Eliminar el registro.";
                return PartialView(etAdminDeudaTutorialDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTutorialDetalleEditar(string EstudianteCI, string Materia)
        {
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
            AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalle = new AdminDeudaTutorialDetalleEntity();

            AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminDeudaTutorialDetalle = setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Materia == Materia).First();

            return PartialView(etAdminDeudaTutorialDetalle);
        }
        [HttpPost]
        public ActionResult AdminDeudaTutorialDetalleEditar(AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalle)
        {
            long NumError = 0;
            AdminDeudaTutorialDetalleEntity etAdminDeudaTutorialDetalleOriginal = etAdminDeudaTutorialDetalle;
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();

            if (NumError == 0)
            {
                //editamos el detalle
                AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTutorialDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etAdminDeudaTutorialDetalleEdit = setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle.Where(x => x.EstudianteCI == etAdminDeudaTutorialDetalle.EstudianteCI && x.Materia == etAdminDeudaTutorialDetalle.Materia).First();

                etAdminDeudaTutorialDetalleEdit.Monto = etAdminDeudaTutorialDetalle.Monto;
                etAdminDeudaTutorialDetalleEdit.NumeroFactura = etAdminDeudaTutorialDetalle.NumeroFactura == null ? "" : etAdminDeudaTutorialDetalle.NumeroFactura;
                etAdminDeudaTutorialDetalleEdit.Concepto = etAdminDeudaTutorialDetalle.Concepto == null ? "" : etAdminDeudaTutorialDetalle.Concepto;
                etAdminDeudaTutorialDetalleEdit.Observaciones = etAdminDeudaTutorialDetalle.Observaciones == null ? "" : etAdminDeudaTutorialDetalle.Observaciones;
                etAdminDeudaTutorialDetalleEdit.UEstado = AdminDeudaTutorialService.Estado.Modified;
                //setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle.Add(etAdminDeudaTutorialDetalle);

                NumError = AdminDeudaTutorial.WebGuardar(setAdminDeudaTutorial, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminDeudaTutorialEditar", "AdminDeudaTutorial", new { EstudianteCI = etAdminDeudaTutorialDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Editar el registro.";
                return PartialView(etAdminDeudaTutorialDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTutorialGestion(string MensajeError = "")
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            List<SelectListItem> Estudiante = new List<SelectListItem>() {
                new SelectListItem() { Value="0", Text="Seleccione un Estudiante" },
           };

            ViewBag.Estudiante = Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View();
        }

        [HttpPost]
        public ActionResult AdminDeudaTutorialGestion(string Estudiante, string MensajeError = "")
        {
            //long NumError = 0;
            //string MensajeError = string.Empty;

            if (Estudiante != null && Estudiante != "")
            {
                return RedirectToAction("AdminDeudaTutorialGestionEstudiante", new { Estudiante = Estudiante });
            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("AdminDeudaTutorialGestion", new { MensajeError = MensajeError });
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTutorialGestionEstudiante(string Estudiante, string MensajeError = "")
        {
            AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
            List<AdminDeudaTutorialDetalleEntity> ltAdminDeudaTutorialDetalle = new List<AdminDeudaTutorialDetalleEntity>();
            if (Estudiante != null && Estudiante != "")
            {
                ltAdminDeudaTutorialDetalle = AdminDeudaTutorial.WebAdminDeudaTutorialDetalleSeleccionarKeyAdminDeudaTutorial((string)Session["SesionSubCompania"], Estudiante, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltAdminDeudaTutorialDetalle = ltAdminDeudaTutorialDetalle.Where(x => x.Pagado == false).ToList();
            }

            ViewBag.EstudianteCI = ltAdminDeudaTutorialDetalle.Count == 0 ? "" : Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View(ltAdminDeudaTutorialDetalle);
        }

        [HttpPost]
        public ActionResult AdminDeudaTutorialGestionEstudiante(List<AdminDeudaTutorialDetalleEntity> ltAdminDeudaTutorialDetalleForm)
        {
            long NumError = 0;
            if (ltAdminDeudaTutorialDetalleForm != null)
            {
                var etAdminDeudaTutorialDetalleForm = ltAdminDeudaTutorialDetalleForm.FirstOrDefault();
                if (etAdminDeudaTutorialDetalleForm != null)
                {
                    AdminDeudaTutorialInterfaceClient AdminDeudaTutorial = new AdminDeudaTutorialInterfaceClient();
                    AdminDeudaTutorialSet setAdminDeudaTutorial = AdminDeudaTutorial.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTutorialDetalleForm.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    var ltAdminDeudaTutorialDetalle = setAdminDeudaTutorial.ltAdminDeudaTutorialDetalle;

                    foreach (var etAdminDeudaTutorialDetalle in ltAdminDeudaTutorialDetalleForm.Where(x => x.Pagado == true))
                    {
                        var ltAdminDeudaTutorialDetalleFiltrado = ltAdminDeudaTutorialDetalle.Where(x => x.Materia == etAdminDeudaTutorialDetalle.Materia).ToList();
                        if (ltAdminDeudaTutorialDetalleFiltrado.Count > 0)
                        {
                            var etAdminDeudaTutorialDetalleFiltrado = ltAdminDeudaTutorialDetalleFiltrado.First();
                            etAdminDeudaTutorialDetalleFiltrado.Pagado = true;
                            etAdminDeudaTutorialDetalleFiltrado.UEstado = AdminDeudaTutorialService.Estado.Modified;
                        }
                    }

                    NumError = AdminDeudaTutorial.WebGuardar(setAdminDeudaTutorial, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }

            if (NumError != 0)
            {
                string MensajeError = "Existio un error al modificar los pagos.";
                return RedirectToAction("AdminDeudaTutorialGestionEstudiante", new { MensajeError = MensajeError });
            }

            return RedirectToAction("AdminDeudaTutorialGestion");
        }

        public JsonResult ObtenerEstudiantesGrupo(string Grupo)
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarXGrupo((string)Session["SesionSubCompania"], Grupo,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo).ToList();

            return Json(new { success = true, ltTASEstudiante = ltTASEstudiante }, JsonRequestBehavior.AllowGet);
        }
    }
}