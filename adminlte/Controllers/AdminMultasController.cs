using adminlte.AdminMultasService;
using adminlte.TASEstudianteService;
using adminlte.TASGrupoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AdminMultasController : BaseController
    {
        // GET: AdminMultas
        public ActionResult AdminMultasLista()
        {
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
            List<AdminMultasEntity> ltAdminMultas = AdminMultas.WebAdminMultasSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminMultas);
        }
        ////Al crear estudiante ya genera una Linea inicial
        //public ActionResult AdminMultasNuevo()
        //{

        //}        

        [HttpGet]
        public ActionResult AdminMultasEditar(string EstudianteCI, string MensajeError = "")
        {
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
            AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminMultas);
        }

        [HttpPost]
        public ActionResult AdminMultasEditar(string EstudianteCI, AdminMultasSet setAdminMultasForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
            AdminMultasEntity etAdminMultasForm = setAdminMultasForm.ltAdminMultas.First();

            if (etAdminMultasForm != null)
            {
                AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AdminMultasEntity etAdminMultas = setAdminMultas.ltAdminMultas.First();
                etAdminMultas.Observacion = etAdminMultasForm.Observacion;
                etAdminMultas.FechaDoc = DateTime.Now;
                etAdminMultas.UEstado = AdminMultasService.Estado.Modified;

                NumError = AdminMultas.WebGuardar(setAdminMultas, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

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
                return RedirectToAction("AdminMultasLista");
            }
            else
            {
                return RedirectToAction("AdminMultasEditar", new { EstudianteCI = EstudianteCI, MensajeError = MensajeError });
            }
        }

        [HttpGet]
        public ActionResult AdminMultasDetalleNuevo(string EstudianteCI, string MensajeError = "")
        {
            AdminMultasDetalleEntity etAdminMultasDetalle = new AdminMultasDetalleEntity();
            etAdminMultasDetalle.SubCompania = (string)Session["SesionSubCompania"];
            etAdminMultasDetalle.EstudianteCI = EstudianteCI;
            etAdminMultasDetalle.Linea = 0;
            etAdminMultasDetalle.FechaDoc = DateTime.Now;
            etAdminMultasDetalle.Monto = 0;
            etAdminMultasDetalle.NumeroFactura = string.Empty;
            etAdminMultasDetalle.FechaPago = DateTime.Now;
            etAdminMultasDetalle.Concepto = string.Empty;
            etAdminMultasDetalle.Pagado = false;
            etAdminMultasDetalle.Observaciones = string.Empty;
            etAdminMultasDetalle.UEstado = AdminMultasService.Estado.Added;
                        
            ViewBag.MensajeError = MensajeError;

            return PartialView(etAdminMultasDetalle);
        }

        [HttpPost]
        public ActionResult AdminMultasDetalleNuevo(AdminMultasDetalleEntity etAdminMultasDetalle)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminMultasDetalleEntity etAdminMultasDetalleOriginal = etAdminMultasDetalle;
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();

            //if (etAdminMultasDetalle.Linea == 0)
            //{
            //    NumError = 1;
            //    MensajeError = "Debe ingresar una Linea valida.";
            //}

            //if (AdminMultas.WebAdminMultasDetalleExiste((string)Session["SesionSubCompania"], etAdminMultasDetalle.EstudianteCI, etAdminMultasDetalle.Linea, (string)Session["Sesion"], (string)Session["SesionSubCompania"]))
            //{
            //    NumError = 1;
            //    MensajeError += " La Linea que esta tratando de Adicionar ya esta siendo usada.";
            //}

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMultasDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminMultasDetalle.SubCompania = (string)Session["SesionSubCompania"];
                etAdminMultasDetalle.Linea = DateTime.Now.Ticks;
                etAdminMultasDetalle.FechaDoc = DateTime.Now;
                etAdminMultasDetalle.Pagado = false;

                etAdminMultasDetalle.NumeroFactura = etAdminMultasDetalle.NumeroFactura == null ? "" : etAdminMultasDetalle.NumeroFactura;
                etAdminMultasDetalle.Concepto = etAdminMultasDetalle.Concepto == null ? "" : etAdminMultasDetalle.Concepto;
                etAdminMultasDetalle.Observaciones = etAdminMultasDetalle.Observaciones == null ? "" : etAdminMultasDetalle.Observaciones;
                etAdminMultasDetalle.UEstado = AdminMultasService.Estado.Added;
                setAdminMultas.ltAdminMultasDetalle.Add(etAdminMultasDetalle);

                NumError = AdminMultas.WebGuardar(setAdminMultas, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminMultasEditar", "AdminMultas", new { EstudianteCI = etAdminMultasDetalle.EstudianteCI }) });
            }
            else
            {
                MensajeError += " Existio un error al guardar el registro.";
                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminMultasDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminMultasDetalleEliminar(string EstudianteCI, long Linea)
        {
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
            AdminMultasDetalleEntity etAdminMultasDetalle = new AdminMultasDetalleEntity();

            AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminMultasDetalle = setAdminMultas.ltAdminMultasDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Linea == Linea).First();

            return PartialView(etAdminMultasDetalle);
        }

        [HttpPost]
        public ActionResult AdminMultasDetalleEliminar(AdminMultasDetalleEntity etAdminMultasDetalle)
        {
            long NumError = 0;
            AdminMultasDetalleEntity etAdminMultasDetalleOriginal = etAdminMultasDetalle;
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMultasDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminMultasDetalle = setAdminMultas.ltAdminMultasDetalle.Where(x => x.EstudianteCI == etAdminMultasDetalle.EstudianteCI && x.Linea == etAdminMultasDetalle.Linea).First();
                etAdminMultasDetalle.UEstado = AdminMultasService.Estado.Deleted;
                setAdminMultas.ltAdminMultasDetalleEliminado.Add(etAdminMultasDetalle);
                etAdminMultasDetalleOriginal = etAdminMultasDetalle;

                NumError = AdminMultas.WebGuardar(setAdminMultas, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminMultasEditar", "AdminMultas", new { EstudianteCI = etAdminMultasDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Eliminar el registro.";
                return PartialView(etAdminMultasDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminMultasDetalleEditar(string EstudianteCI, long Linea)
        {
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
            AdminMultasDetalleEntity etAdminMultasDetalle = new AdminMultasDetalleEntity();

            AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminMultasDetalle = setAdminMultas.ltAdminMultasDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Linea == Linea).First();

            return PartialView(etAdminMultasDetalle);
        }
        [HttpPost]
        public ActionResult AdminMultasDetalleEditar(AdminMultasDetalleEntity etAdminMultasDetalle)
        {
            long NumError = 0;
            AdminMultasDetalleEntity etAdminMultasDetalleOriginal = etAdminMultasDetalle;
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();

            if (NumError == 0)
            {
                //editamos el detalle
                AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMultasDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etAdminMultasDetalleEdit = setAdminMultas.ltAdminMultasDetalle.Where(x => x.EstudianteCI == etAdminMultasDetalle.EstudianteCI && x.Linea == etAdminMultasDetalle.Linea).First();

                etAdminMultasDetalleEdit.Monto = etAdminMultasDetalle.Monto;
                etAdminMultasDetalleEdit.NumeroFactura = etAdminMultasDetalle.NumeroFactura == null ? "" : etAdminMultasDetalle.NumeroFactura;
                etAdminMultasDetalleEdit.Concepto = etAdminMultasDetalle.Concepto == null ? "" : etAdminMultasDetalle.Concepto;
                etAdminMultasDetalleEdit.Observaciones = etAdminMultasDetalle.Observaciones == null ? "" : etAdminMultasDetalle.Observaciones;
                etAdminMultasDetalleEdit.UEstado = AdminMultasService.Estado.Modified;
                //setAdminMultas.ltAdminMultasDetalle.Add(etAdminMultasDetalle);

                NumError = AdminMultas.WebGuardar(setAdminMultas, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminMultasEditar", "AdminMultas", new { EstudianteCI = etAdminMultasDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Editar el registro.";
                return PartialView(etAdminMultasDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminMultasGestion(string MensajeError = "")
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
        public ActionResult AdminMultasGestion(string Estudiante, string MensajeError = "")
        {
            //long NumError = 0;
            //string MensajeError = string.Empty;

            if (Estudiante != null && Estudiante != "")
            {
                return RedirectToAction("AdminMultasGestionEstudiante", new { Estudiante = Estudiante });
            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("AdminMultasGestion", new { MensajeError = MensajeError });
            }

        }

        [HttpGet]
        public ActionResult AdminMultasGestionEstudiante(string Estudiante, string MensajeError = "")
        {
            AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
            List<AdminMultasDetalleEntity> ltAdminMultasDetalle = new List<AdminMultasDetalleEntity>();
            if (Estudiante != null && Estudiante != "")
            {
                ltAdminMultasDetalle = AdminMultas.WebAdminMultasDetalleSeleccionarKeyAdminMultas((string)Session["SesionSubCompania"], Estudiante, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltAdminMultasDetalle = ltAdminMultasDetalle.Where(x => x.Pagado == false).ToList();
            }

            ViewBag.EstudianteCI = ltAdminMultasDetalle.Count == 0 ? "" : Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View(ltAdminMultasDetalle);
        }

        [HttpPost]
        public ActionResult AdminMultasGestionEstudiante(List<AdminMultasDetalleEntity> ltAdminMultasDetalleForm)
        {
            long NumError = 0;
            if (ltAdminMultasDetalleForm != null)
            {
                var etAdminMultasDetalleForm = ltAdminMultasDetalleForm.FirstOrDefault();
                if (etAdminMultasDetalleForm != null)
                {
                    AdminMultasInterfaceClient AdminMultas = new AdminMultasInterfaceClient();
                    AdminMultasSet setAdminMultas = AdminMultas.WebSeleccionar((string)Session["SesionSubCompania"], etAdminMultasDetalleForm.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    var ltAdminMultasDetalle = setAdminMultas.ltAdminMultasDetalle;

                    foreach (var etAdminMultasDetalle in ltAdminMultasDetalleForm.Where(x => x.Pagado == true))
                    {
                        var ltAdminMultasDetalleFiltrado = ltAdminMultasDetalle.Where(x => x.Linea == etAdminMultasDetalle.Linea).ToList();
                        if (ltAdminMultasDetalleFiltrado.Count > 0)
                        {
                            var etAdminMultasDetalleFiltrado = ltAdminMultasDetalleFiltrado.First();
                            etAdminMultasDetalleFiltrado.Pagado = true;
                            etAdminMultasDetalleFiltrado.UEstado = AdminMultasService.Estado.Modified;
                        }
                    }

                    NumError = AdminMultas.WebGuardar(setAdminMultas, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }

            if (NumError != 0)
            {
                string MensajeError = "Existio un error al modificar los pagos.";
                return RedirectToAction("AdminMultasGestionEstudiante", new { MensajeError = MensajeError });
            }

            return RedirectToAction("AdminMultasGestion");
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