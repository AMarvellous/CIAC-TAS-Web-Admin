using adminlte.AdminMensualidadService;
using adminlte.AdminCatalogoMensualidadService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using adminlte.TASGrupoService;
using adminlte.TASEstudianteService;

namespace adminlte.Controllers
{
    public class AdminMensualidadController : BaseController
    {
        // GET: AdminMensualidad
        public ActionResult AdminMensualidadLista()
        {
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
            List<AdminMensualidadEntity> ltAdminMensualidad = AdminMensualidad.WebAdminMensualidadSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            
            return View(ltAdminMensualidad);
        }
        ////Al crear estudiante ya genera una mensualidad inicial
        //public ActionResult AdminMensualidadNuevo()
        //{

        //}        

        [HttpGet]
        public ActionResult AdminMensualidadEditar( string EstudianteCI, string MensajeError = "")
        {
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
            AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminMensualidad);
        }

        [HttpPost]
        public ActionResult AdminMensualidadEditar(string EstudianteCI, AdminMensualidadSet setAdminMensualidadForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
            AdminMensualidadEntity etAdminMensualidadForm = setAdminMensualidadForm.ltAdminMensualidad.First();
            
            if (etAdminMensualidadForm != null)
            {
                AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AdminMensualidadEntity etAdminMensualidad = setAdminMensualidad.ltAdminMensualidad.First();
                etAdminMensualidad.Observacion = etAdminMensualidadForm.Observacion;
                etAdminMensualidad.FechaDoc = DateTime.Now;
                etAdminMensualidad.UEstado = AdminMensualidadService.Estado.Modified;

                NumError = AdminMensualidad.WebGuardar(setAdminMensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                
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
                return RedirectToAction("AdminMensualidadLista");
            }
            else
            {
                return RedirectToAction("AdminMensualidadEditar", new { EstudianteCI = EstudianteCI, MensajeError = MensajeError });
            }
        }

        [HttpGet]
        public ActionResult AdminMensualidadDetalleNuevo(string EstudianteCI, string MensajeError = "")
        {
            AdminMensualidadDetalleEntity etAdminMensualidadDetalle = new AdminMensualidadDetalleEntity();
            etAdminMensualidadDetalle.SubCompania = (string)Session["SesionSubCompania"];
            etAdminMensualidadDetalle.EstudianteCI = EstudianteCI;
            etAdminMensualidadDetalle.Mensualidad = string.Empty;
            etAdminMensualidadDetalle.FechaDoc = DateTime.Now;
            etAdminMensualidadDetalle.Monto = 0;
            etAdminMensualidadDetalle.NumeroFactura = string.Empty;
            etAdminMensualidadDetalle.FechaPago = DateTime.Now;
            etAdminMensualidadDetalle.Concepto = string.Empty;
            etAdminMensualidadDetalle.Pagado = false;
            etAdminMensualidadDetalle.Observaciones = string.Empty;
            etAdminMensualidadDetalle.UEstado = AdminMensualidadService.Estado.Added;

            AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
            List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltAdminCatalogoMensualidad = ltAdminCatalogoMensualidad.Select(s => s.Mensualidad).ToList();

            ViewBag.MensajeError = MensajeError;

            return PartialView(etAdminMensualidadDetalle);
        }

        [HttpPost]
        public ActionResult AdminMensualidadDetalleNuevo(AdminMensualidadDetalleEntity etAdminMensualidadDetalle)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminMensualidadDetalleEntity etAdminMensualidadDetalleOriginal = etAdminMensualidadDetalle;
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();

            if (etAdminMensualidadDetalle.Mensualidad ==  null || etAdminMensualidadDetalle.Mensualidad == string.Empty)
            {
                NumError = 1;
                MensajeError = "Debe ingresar una mensualidad valida.";
            }

            if (AdminMensualidad.WebAdminMensualidadDetalleExiste((string)Session["SesionSubCompania"], etAdminMensualidadDetalle.EstudianteCI, etAdminMensualidadDetalle.Mensualidad, (string)Session["Sesion"], (string)Session["SesionSubCompania"]))
            {
                NumError = 1;
                MensajeError += " La mensualidad que esta tratando de Adicionar ya esta siendo usada.";
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMensualidadDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminMensualidadDetalle.SubCompania = (string)Session["SesionSubCompania"];
                etAdminMensualidadDetalle.FechaDoc = DateTime.Now;
                etAdminMensualidadDetalle.Pagado = false;

                etAdminMensualidadDetalle.NumeroFactura = etAdminMensualidadDetalle.NumeroFactura == null ? "" : etAdminMensualidadDetalle.NumeroFactura;
                etAdminMensualidadDetalle.Concepto = etAdminMensualidadDetalle.Concepto == null ? "" : etAdminMensualidadDetalle.Concepto;
                etAdminMensualidadDetalle.Observaciones = etAdminMensualidadDetalle.Observaciones == null ? "" : etAdminMensualidadDetalle.Observaciones;
                etAdminMensualidadDetalle.UEstado = AdminMensualidadService.Estado.Added;
                setAdminMensualidad.ltAdminMensualidadDetalle.Add(etAdminMensualidadDetalle);

                NumError = AdminMensualidad.WebGuardar(setAdminMensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminMensualidadEditar", "AdminMensualidad", new { EstudianteCI = etAdminMensualidadDetalle.EstudianteCI }) });
            }
            else
            {
                AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
                List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltAdminCatalogoMensualidad = ltAdminCatalogoMensualidad.Select(s => s.Mensualidad).ToList();
                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminMensualidadDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminMensualidadDetalleEliminar(string EstudianteCI, string Mensualidad)
        {
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
            AdminMensualidadDetalleEntity etAdminMensualidadDetalle = new AdminMensualidadDetalleEntity();

            AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminMensualidadDetalle = setAdminMensualidad.ltAdminMensualidadDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Mensualidad == Mensualidad).First();
            
            return PartialView(etAdminMensualidadDetalle);
        }

        [HttpPost]
        public ActionResult AdminMensualidadDetalleEliminar(AdminMensualidadDetalleEntity etAdminMensualidadDetalle)
        {
            long NumError = 0;
            AdminMensualidadDetalleEntity etAdminMensualidadDetalleOriginal = etAdminMensualidadDetalle;
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMensualidadDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminMensualidadDetalle = setAdminMensualidad.ltAdminMensualidadDetalle.Where(x => x.EstudianteCI == etAdminMensualidadDetalle.EstudianteCI && x.Mensualidad == etAdminMensualidadDetalle.Mensualidad).First();
                etAdminMensualidadDetalle.UEstado = AdminMensualidadService.Estado.Deleted;
                setAdminMensualidad.ltAdminMensualidadDetalleEliminado.Add(etAdminMensualidadDetalle);
                etAdminMensualidadDetalleOriginal = etAdminMensualidadDetalle;

                NumError = AdminMensualidad.WebGuardar(setAdminMensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminMensualidadEditar", "AdminMensualidad", new { EstudianteCI = etAdminMensualidadDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Eliminar el registro.";
                return PartialView(etAdminMensualidadDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminMensualidadDetalleEditar(string EstudianteCI, string Mensualidad)
        {
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
            AdminMensualidadDetalleEntity etAdminMensualidadDetalle = new AdminMensualidadDetalleEntity();

            AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminMensualidadDetalle = setAdminMensualidad.ltAdminMensualidadDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Mensualidad == Mensualidad).First();
            
            return PartialView(etAdminMensualidadDetalle);
        }
        [HttpPost]
        public ActionResult AdminMensualidadDetalleEditar(AdminMensualidadDetalleEntity etAdminMensualidadDetalle)
        {
            long NumError = 0;
            AdminMensualidadDetalleEntity etAdminMensualidadDetalleOriginal = etAdminMensualidadDetalle;
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();

            if (NumError == 0)
            {
                //editamos el detalle
                AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMensualidadDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etAdminMensualidadDetalleEdit = setAdminMensualidad.ltAdminMensualidadDetalle.Where(x => x.EstudianteCI == etAdminMensualidadDetalle.EstudianteCI && x.Mensualidad == etAdminMensualidadDetalle.Mensualidad).First();

                etAdminMensualidadDetalleEdit.Monto = etAdminMensualidadDetalle.Monto;
                etAdminMensualidadDetalleEdit.NumeroFactura = etAdminMensualidadDetalle.NumeroFactura == null ? "" : etAdminMensualidadDetalle.NumeroFactura;
                etAdminMensualidadDetalleEdit.Concepto = etAdminMensualidadDetalle.Concepto == null ? "" : etAdminMensualidadDetalle.Concepto;
                etAdminMensualidadDetalleEdit.Observaciones = etAdminMensualidadDetalle.Observaciones == null ? "" : etAdminMensualidadDetalle.Observaciones;
                etAdminMensualidadDetalleEdit.UEstado = AdminMensualidadService.Estado.Modified;
                //setAdminMensualidad.ltAdminMensualidadDetalle.Add(etAdminMensualidadDetalle);

                NumError = AdminMensualidad.WebGuardar(setAdminMensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminMensualidadEditar", "AdminMensualidad", new { EstudianteCI = etAdminMensualidadDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Editar el registro.";
                return PartialView(etAdminMensualidadDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminMensualidadGestion(string MensajeError = "")
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
        public ActionResult AdminMensualidadGestion(string Estudiante, string MensajeError = "")
        {
            //long NumError = 0;
            //string MensajeError = string.Empty;

            if (Estudiante != null && Estudiante != "")
            {
                return RedirectToAction("AdminMensualidadGestionEstudiante", new { Estudiante = Estudiante });
            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("AdminMensualidadGestion", new { MensajeError = MensajeError });
            }

        }

        [HttpGet]
        public ActionResult AdminMensualidadGestionEstudiante(string Estudiante, string MensajeError = "")
        {
            AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
            List<AdminMensualidadDetalleEntity> ltAdminMensualidadDetalle = new List<AdminMensualidadDetalleEntity>();
            if (Estudiante != null && Estudiante != "")
            {
                ltAdminMensualidadDetalle = AdminMensualidad.WebAdminMensualidadDetalleSeleccionarKeyAdminMensualidad((string)Session["SesionSubCompania"], Estudiante, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltAdminMensualidadDetalle = ltAdminMensualidadDetalle.Where(x => x.Pagado == false).ToList();
            }

            ViewBag.EstudianteCI = ltAdminMensualidadDetalle.Count == 0 ? "" : Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View(ltAdminMensualidadDetalle);
        }

        [HttpPost]
        public ActionResult AdminMensualidadGestionEstudiante(List<AdminMensualidadDetalleEntity> ltAdminMensualidadDetalleForm)
        {
            long NumError = 0;
            if (ltAdminMensualidadDetalleForm != null)
            {
                var etAdminMensualidadDetalleForm = ltAdminMensualidadDetalleForm.FirstOrDefault();
                if (etAdminMensualidadDetalleForm != null)
                {
                    AdminMensualidadInterfaceClient AdminMensualidad = new AdminMensualidadInterfaceClient();
                    AdminMensualidadSet setAdminMensualidad = AdminMensualidad.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMensualidadDetalleForm.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    var ltAdminMensualidadDetalle = setAdminMensualidad.ltAdminMensualidadDetalle;

                    foreach (var etAdminMensualidadDetalle in ltAdminMensualidadDetalleForm.Where(x => x.Pagado == true))
                    {
                        var ltAdminMensualidadDetalleFiltrado = ltAdminMensualidadDetalle.Where(x => x.Mensualidad == etAdminMensualidadDetalle.Mensualidad).ToList();
                        if (ltAdminMensualidadDetalleFiltrado.Count > 0)
                        {
                            var etAdminMensualidadDetalleFiltrado = ltAdminMensualidadDetalleFiltrado.First();
                            etAdminMensualidadDetalleFiltrado.Pagado = true;
                            etAdminMensualidadDetalleFiltrado.UEstado = AdminMensualidadService.Estado.Modified;
                        }
                    }

                    NumError = AdminMensualidad.WebGuardar(setAdminMensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }                
            }

            if (NumError != 0)
            {
                string MensajeError = "Existio un error al modificar los pagos.";
                return RedirectToAction("AdminMensualidadGestionEstudiante", new { MensajeError = MensajeError});
            }

            return RedirectToAction("AdminMensualidadGestion");           
        }

        public JsonResult ObtenerEstudiantesGrupo(string Grupo)
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarXGrupo((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo).ToList();

            return Json(new { success = true, ltTASEstudiante = ltTASEstudiante }, JsonRequestBehavior.AllowGet);
        }
    }
}