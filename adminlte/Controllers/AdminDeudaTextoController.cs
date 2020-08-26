using adminlte.AdminCatalogoTextoService;
using adminlte.AdminDeudaTextoService;
using adminlte.TASEstudianteService;
using adminlte.TASGrupoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using adminlte.Classes;

namespace adminlte.Controllers
{
    public class AdminDeudaTextoController : BaseController
    {
        // GET: AdminDeudaTexto
        public ActionResult AdminDeudaTextoLista()
        {
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
            List<AdminDeudaTextoEntity> ltAdminDeudaTexto = AdminDeudaTexto.WebAdminDeudaTextoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminDeudaTexto);
        }
        ////Al crear estudiante ya genera una Texto inicial
        //public ActionResult AdminDeudaTextoNuevo()
        //{

        //}        

        [HttpGet]
        public ActionResult AdminDeudaTextoEditar(string EstudianteCI, string MensajeError = "")
        {
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
            AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminDeudaTexto);
        }

        [HttpPost]
        public ActionResult AdminDeudaTextoEditar(string EstudianteCI, AdminDeudaTextoSet setAdminDeudaTextoForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
            AdminDeudaTextoEntity etAdminDeudaTextoForm = setAdminDeudaTextoForm.ltAdminDeudaTexto.First();

            if (etAdminDeudaTextoForm != null)
            {
                AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AdminDeudaTextoEntity etAdminDeudaTexto = setAdminDeudaTexto.ltAdminDeudaTexto.First();
                etAdminDeudaTexto.Observacion = etAdminDeudaTextoForm.Observacion;
                etAdminDeudaTexto.FechaDoc = DateTime.Now;
                etAdminDeudaTexto.UEstado = AdminDeudaTextoService.Estado.Modified;

                NumError = AdminDeudaTexto.WebGuardar(setAdminDeudaTexto, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

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
                return RedirectToAction("AdminDeudaTextoLista");
            }
            else
            {
                return RedirectToAction("AdminDeudaTextoEditar", new { EstudianteCI = EstudianteCI, MensajeError = MensajeError });
            }
        }

        [HttpGet]
        public ActionResult AdminDeudaTextoDetalleNuevo(string EstudianteCI, string MensajeError = "")
        {            
            AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalle = new AdminDeudaTextoDetalleEntity();
            etAdminDeudaTextoDetalle.SubCompania = (string)Session["SesionSubCompania"];
            etAdminDeudaTextoDetalle.EstudianteCI = EstudianteCI;            
            etAdminDeudaTextoDetalle.Texto = string.Empty;
            etAdminDeudaTextoDetalle.FechaDoc = DateTime.Now;
            etAdminDeudaTextoDetalle.Monto = 0;
            etAdminDeudaTextoDetalle.NumeroFactura = string.Empty;
            etAdminDeudaTextoDetalle.FechaPago = DateTime.Now;
            etAdminDeudaTextoDetalle.Concepto = string.Empty;
            etAdminDeudaTextoDetalle.Pagado = false;
            etAdminDeudaTextoDetalle.Observaciones = string.Empty;
            etAdminDeudaTextoDetalle.UEstado = AdminDeudaTextoService.Estado.Added;

            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
            List<AdminCatalogoTextoEntity> ltAdminCatalogoTexto = AdminCatalogoTexto.WebAdminCatalogoTextoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltAdminCatalogoTexto = ltAdminCatalogoTexto.Select(s => s.Texto).ToList();

            ViewBag.MensajeError = MensajeError;

            return PartialView(etAdminDeudaTextoDetalle);
        }

        [HttpPost]
        public ActionResult AdminDeudaTextoDetalleNuevo(AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalle)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalleOriginal = etAdminDeudaTextoDetalle;
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();

            if (etAdminDeudaTextoDetalle.Texto == null || etAdminDeudaTextoDetalle.Texto == string.Empty)
            {
                NumError = 1;
                MensajeError = "Debe ingresar un Texto valida.";
            }

            if (AdminDeudaTexto.WebAdminDeudaTextoDetalleExiste((string)Session["SesionSubCompania"], etAdminDeudaTextoDetalle.EstudianteCI, etAdminDeudaTextoDetalle.Texto, (string)Session["Sesion"], (string)Session["SesionSubCompania"]))
            {
                NumError = 1;
                MensajeError += " El Texto que esta tratando de Adicionar ya esta siendo usada.";
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTextoDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminDeudaTextoDetalle.SubCompania = (string)Session["SesionSubCompania"];
                etAdminDeudaTextoDetalle.FechaDoc = DateTime.Now;
                etAdminDeudaTextoDetalle.Pagado = false;

                etAdminDeudaTextoDetalle.NumeroFactura = etAdminDeudaTextoDetalle.NumeroFactura == null ? "" : etAdminDeudaTextoDetalle.NumeroFactura;
                etAdminDeudaTextoDetalle.Concepto = etAdminDeudaTextoDetalle.Concepto == null ? "" : etAdminDeudaTextoDetalle.Concepto;
                etAdminDeudaTextoDetalle.Observaciones = etAdminDeudaTextoDetalle.Observaciones == null ? "" : etAdminDeudaTextoDetalle.Observaciones;
                etAdminDeudaTextoDetalle.UEstado = AdminDeudaTextoService.Estado.Added;
                setAdminDeudaTexto.ltAdminDeudaTextoDetalle.Add(etAdminDeudaTextoDetalle);

                NumError = AdminDeudaTexto.WebGuardar(setAdminDeudaTexto, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminDeudaTextoEditar", "AdminDeudaTexto", new { EstudianteCI = etAdminDeudaTextoDetalle.EstudianteCI }) });
            }
            else
            {
                AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
                List<AdminCatalogoTextoEntity> ltAdminCatalogoTexto = AdminCatalogoTexto.WebAdminCatalogoTextoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltAdminCatalogoTexto = ltAdminCatalogoTexto.Select(s => s.Texto).ToList();
                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminDeudaTextoDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTextoDetalleEliminar(string EstudianteCI, string Texto)
        {
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
            AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalle = new AdminDeudaTextoDetalleEntity();

            AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminDeudaTextoDetalle = setAdminDeudaTexto.ltAdminDeudaTextoDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Texto == Texto).First();

            return PartialView(etAdminDeudaTextoDetalle);
        }

        [HttpPost]
        public ActionResult AdminDeudaTextoDetalleEliminar(AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalle)
        {
            long NumError = 0;
            AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalleOriginal = etAdminDeudaTextoDetalle;
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTextoDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminDeudaTextoDetalle = setAdminDeudaTexto.ltAdminDeudaTextoDetalle.Where(x => x.EstudianteCI == etAdminDeudaTextoDetalle.EstudianteCI && x.Texto == etAdminDeudaTextoDetalle.Texto).First();
                etAdminDeudaTextoDetalle.UEstado = AdminDeudaTextoService.Estado.Deleted;
                setAdminDeudaTexto.ltAdminDeudaTextoDetalleEliminado.Add(etAdminDeudaTextoDetalle);
                etAdminDeudaTextoDetalleOriginal = etAdminDeudaTextoDetalle;

                NumError = AdminDeudaTexto.WebGuardar(setAdminDeudaTexto, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminDeudaTextoEditar", "AdminDeudaTexto", new { EstudianteCI = etAdminDeudaTextoDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Eliminar el registro.";
                return PartialView(etAdminDeudaTextoDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTextoDetalleEditar(string EstudianteCI, string Texto)
        {
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
            AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalle = new AdminDeudaTextoDetalleEntity();

            AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminDeudaTextoDetalle = setAdminDeudaTexto.ltAdminDeudaTextoDetalle.Where(x => x.EstudianteCI == EstudianteCI && x.Texto == Texto).First();

            return PartialView(etAdminDeudaTextoDetalle);
        }
        [HttpPost]
        public ActionResult AdminDeudaTextoDetalleEditar(AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalle)
        {
            long NumError = 0;
            AdminDeudaTextoDetalleEntity etAdminDeudaTextoDetalleOriginal = etAdminDeudaTextoDetalle;
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();

            if (NumError == 0)
            {
                //editamos el detalle
                AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTextoDetalle.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etAdminDeudaTextoDetalleEdit = setAdminDeudaTexto.ltAdminDeudaTextoDetalle.Where(x => x.EstudianteCI == etAdminDeudaTextoDetalle.EstudianteCI && x.Texto == etAdminDeudaTextoDetalle.Texto).First();

                etAdminDeudaTextoDetalleEdit.Monto = etAdminDeudaTextoDetalle.Monto;
                etAdminDeudaTextoDetalleEdit.NumeroFactura = etAdminDeudaTextoDetalle.NumeroFactura == null ? "" : etAdminDeudaTextoDetalle.NumeroFactura;
                etAdminDeudaTextoDetalleEdit.Concepto = etAdminDeudaTextoDetalle.Concepto == null ? "" : etAdminDeudaTextoDetalle.Concepto;
                etAdminDeudaTextoDetalleEdit.Observaciones = etAdminDeudaTextoDetalle.Observaciones == null ? "" : etAdminDeudaTextoDetalle.Observaciones;
                etAdminDeudaTextoDetalleEdit.UEstado = AdminDeudaTextoService.Estado.Modified;
                //setAdminDeudaTexto.ltAdminDeudaTextoDetalle.Add(etAdminDeudaTextoDetalle);

                NumError = AdminDeudaTexto.WebGuardar(setAdminDeudaTexto, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminDeudaTextoEditar", "AdminDeudaTexto", new { EstudianteCI = etAdminDeudaTextoDetalle.EstudianteCI }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Editar el registro.";
                return PartialView(etAdminDeudaTextoDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTextoGestion(string MensajeError = "")
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
        public ActionResult AdminDeudaTextoGestion(string Estudiante, string MensajeError = "")
        {
            //long NumError = 0;
            //string MensajeError = string.Empty;

            if (Estudiante != null && Estudiante != "")
            {
                return RedirectToAction("AdminDeudaTextoGestionEstudiante", new { Estudiante = Estudiante });
            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("AdminDeudaTextoGestion", new { MensajeError = MensajeError });
            }

        }

        [HttpGet]
        public ActionResult AdminDeudaTextoGestionEstudiante(string Estudiante, string MensajeError = "")
        {
            AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
            List<AdminDeudaTextoDetalleEntity> ltAdminDeudaTextoDetalle = new List<AdminDeudaTextoDetalleEntity>();
            if (Estudiante != null && Estudiante != "")
            {
                ltAdminDeudaTextoDetalle = AdminDeudaTexto.WebAdminDeudaTextoDetalleSeleccionarKeyAdminDeudaTexto((string)Session["SesionSubCompania"], Estudiante, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltAdminDeudaTextoDetalle = ltAdminDeudaTextoDetalle.Where(x => x.Pagado == false).ToList();
            }

            ViewBag.EstudianteCI = ltAdminDeudaTextoDetalle.Count == 0 ? "" : Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View(ltAdminDeudaTextoDetalle);
        }

        [HttpPost]
        public ActionResult AdminDeudaTextoGestionEstudiante(List<AdminDeudaTextoDetalleEntity> ltAdminDeudaTextoDetalleForm)
        {
            long NumError = 0;
            if (ltAdminDeudaTextoDetalleForm != null)
            {
                var etAdminDeudaTextoDetalleForm = ltAdminDeudaTextoDetalleForm.FirstOrDefault();
                if (etAdminDeudaTextoDetalleForm != null)
                {
                    AdminDeudaTextoInterfaceClient AdminDeudaTexto = new AdminDeudaTextoInterfaceClient();
                    AdminDeudaTextoSet setAdminDeudaTexto = AdminDeudaTexto.WebSeleccionar((string)Session["SesionSubCompania"], etAdminDeudaTextoDetalleForm.EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    var ltAdminDeudaTextoDetalle = setAdminDeudaTexto.ltAdminDeudaTextoDetalle;

                    foreach (var etAdminDeudaTextoDetalle in ltAdminDeudaTextoDetalleForm.Where(x => x.Pagado == true))
                    {
                        var ltAdminDeudaTextoDetalleFiltrado = ltAdminDeudaTextoDetalle.Where(x => x.Texto == etAdminDeudaTextoDetalle.Texto).ToList();
                        if (ltAdminDeudaTextoDetalleFiltrado.Count > 0)
                        {
                            var etAdminDeudaTextoDetalleFiltrado = ltAdminDeudaTextoDetalleFiltrado.First();
                            etAdminDeudaTextoDetalleFiltrado.Pagado = true;
                            etAdminDeudaTextoDetalleFiltrado.UEstado = AdminDeudaTextoService.Estado.Modified;
                        }
                    }

                    NumError = AdminDeudaTexto.WebGuardar(setAdminDeudaTexto, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }

            if (NumError != 0)
            {
                string MensajeError = "Existio un error al modificar los pagos.";
                return RedirectToAction("AdminDeudaTextoGestionEstudiante", new { MensajeError = MensajeError });
            }

            return RedirectToAction("AdminDeudaTextoGestion");
        }

        public JsonResult ObtenerEstudiantesGrupo(string Grupo)
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarXGrupo((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo).ToList();

            return Json(new { success = true, ltTASEstudiante = ltTASEstudiante }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerMontoSugerido(string Texto)
        {
            AdminCatalogoTextoHelper adminCatalogoTextoHelper = new AdminCatalogoTextoHelper();
            var MontoSugerido = adminCatalogoTextoHelper.GetMontoSugerido((string)Session["SesionSubCompania"], Texto, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            
            return Json(new { success = true, MontoSugerido = MontoSugerido }, JsonRequestBehavior.AllowGet);
        }
    }
}