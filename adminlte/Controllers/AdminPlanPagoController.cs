using adminlte.AdminCatalogoMensualidadService;
using adminlte.AdminPlanPagoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AdminPlanPagoController : BaseController
    {
        // GET: AdminPlanPago
        public ActionResult AdminPlanPagoLista()
        {
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();
            List<AdminPlanPagoEntity> ltAdminPlanPago = AdminPlanPago.WebAdminPlanPagoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminPlanPago);
        }
        ////Al crear Grupo ya genera una Plan de pago para ese grupo
        //public ActionResult AdminPlanPagoNuevo()
        //{

        //}        

        [HttpGet]
        public ActionResult AdminPlanPagoEditar(string Grupo, string MensajeError = "")
        {
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();
            AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminPlanPago);
        }

        [HttpPost]
        public ActionResult AdminPlanPagoEditar(string Grupo, AdminPlanPagoSet setAdminPlanPagoForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();
            AdminPlanPagoEntity etAdminPlanPagoForm = setAdminPlanPagoForm.ltAdminPlanPago.First();

            if (etAdminPlanPagoForm != null)
            {
                AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AdminPlanPagoEntity etAdminPlanPago = setAdminPlanPago.ltAdminPlanPago.First();
                etAdminPlanPago.Descripcion = etAdminPlanPagoForm.Descripcion;
                etAdminPlanPago.Observacion = etAdminPlanPagoForm.Observacion;
                etAdminPlanPago.UEstado = AdminPlanPagoService.Estado.Modified;

                NumError = AdminPlanPago.WebGuardar(setAdminPlanPago, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

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
                return RedirectToAction("AdminPlanPagoLista");
            }
            else
            {
                return RedirectToAction("AdminPlanPagoEditar", new { Grupo = Grupo, MensajeError = MensajeError });
            }
        }

        [HttpGet]
        public ActionResult AdminPlanPagoDetalleNuevo(string Grupo, string MensajeError = "")
        {
            AdminPlanPagoDetalleEntity etAdminPlanPagoDetalle = new AdminPlanPagoDetalleEntity();
            etAdminPlanPagoDetalle.SubCompania = (string)Session["SesionSubCompania"];
            etAdminPlanPagoDetalle.Grupo = Grupo;
            etAdminPlanPagoDetalle.Linea = 0;
            etAdminPlanPagoDetalle.FechaLimite = DateTime.Now;
            etAdminPlanPagoDetalle.Mensualidad = string.Empty;
            etAdminPlanPagoDetalle.MontoSugerido = 0;
            etAdminPlanPagoDetalle.UEstado = AdminPlanPagoService.Estado.Added;

            AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
            List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltAdminCatalogoMensualidad = ltAdminCatalogoMensualidad.Select(s => s.Mensualidad).ToList();

            ViewBag.MensajeError = MensajeError;

            return PartialView(etAdminPlanPagoDetalle);
        }

        [HttpPost]
        public ActionResult AdminPlanPagoDetalleNuevo(AdminPlanPagoDetalleEntity etAdminPlanPagoDetalle)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminPlanPagoDetalleEntity etAdminPlanPagoDetalleOriginal = etAdminPlanPagoDetalle;
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();

            if (etAdminPlanPagoDetalle.Mensualidad == null || etAdminPlanPagoDetalle.Mensualidad == string.Empty)
            {
                NumError = 1;
                MensajeError += "Debe ingresar una Mensualidad valida.";
            }

            if (AdminPlanPago.WebAdminPlanPagoDetalleExisteMensualidad((string)Session["SesionSubCompania"], etAdminPlanPagoDetalle.Grupo, etAdminPlanPagoDetalle.Linea, etAdminPlanPagoDetalle.Mensualidad, true , (string)Session["Sesion"], (string)Session["SesionSubCompania"]))
            {
                NumError = 1;
                MensajeError += " La Mensualidad que esta tratando de Adicionar ya esta siendo usada.";
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], etAdminPlanPagoDetalle.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminPlanPagoDetalle.SubCompania = (string)Session["SesionSubCompania"];
                etAdminPlanPagoDetalle.Linea = DateTime.Now.Ticks;

                etAdminPlanPagoDetalle.UEstado = AdminPlanPagoService.Estado.Added;
                setAdminPlanPago.ltAdminPlanPagoDetalle.Add(etAdminPlanPagoDetalle);

                NumError = AdminPlanPago.WebGuardar(setAdminPlanPago, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminPlanPagoEditar", "AdminPlanPago", new { Grupo = etAdminPlanPagoDetalle.Grupo }) });
            }
            else
            {
                AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
                List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltAdminCatalogoMensualidad = ltAdminCatalogoMensualidad.Select(s => s.Mensualidad).ToList();
                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminPlanPagoDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminPlanPagoDetalleEliminar(string Grupo, long Linea)
        {
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();
            AdminPlanPagoDetalleEntity etAdminPlanPagoDetalle = new AdminPlanPagoDetalleEntity();

            AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminPlanPagoDetalle = setAdminPlanPago.ltAdminPlanPagoDetalle.Where(x => x.Grupo == Grupo && x.Linea == Linea).First();

            return PartialView(etAdminPlanPagoDetalle);
        }

        [HttpPost]
        public ActionResult AdminPlanPagoDetalleEliminar(AdminPlanPagoDetalleEntity etAdminPlanPagoDetalle)
        {
            long NumError = 0;
            AdminPlanPagoDetalleEntity etAdminPlanPagoDetalleOriginal = etAdminPlanPagoDetalle;
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], etAdminPlanPagoDetalle.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAdminPlanPagoDetalle = setAdminPlanPago.ltAdminPlanPagoDetalle.Where(x => x.Grupo == etAdminPlanPagoDetalle.Grupo && x.Linea == etAdminPlanPagoDetalle.Linea).First();
                etAdminPlanPagoDetalle.UEstado = AdminPlanPagoService.Estado.Deleted;
                setAdminPlanPago.ltAdminPlanPagoDetalleEliminado.Add(etAdminPlanPagoDetalle);
                etAdminPlanPagoDetalleOriginal = etAdminPlanPagoDetalle;

                NumError = AdminPlanPago.WebGuardar(setAdminPlanPago, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminPlanPagoEditar", "AdminPlanPago", new { Grupo = etAdminPlanPagoDetalle.Grupo }) });
            }
            else
            {
                ViewBag.MensajeError = "Ocurrio un error al tratar de Eliminar el registro.";
                return PartialView(etAdminPlanPagoDetalleOriginal);
            }

        }

        [HttpGet]
        public ActionResult AdminPlanPagoDetalleEditar(string Grupo, long Linea)
        {
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();
            AdminPlanPagoDetalleEntity etAdminPlanPagoDetalle = new AdminPlanPagoDetalleEntity();

            AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etAdminPlanPagoDetalle = setAdminPlanPago.ltAdminPlanPagoDetalle.Where(x => x.Grupo == Grupo && x.Linea == Linea).First();

            AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
            List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltAdminCatalogoMensualidad = ltAdminCatalogoMensualidad.Select(s => s.Mensualidad).ToList();

            return PartialView(etAdminPlanPagoDetalle);
        }
        [HttpPost]
        public ActionResult AdminPlanPagoDetalleEditar(AdminPlanPagoDetalleEntity etAdminPlanPagoDetalle)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminPlanPagoDetalleEntity etAdminPlanPagoDetalleOriginal = etAdminPlanPagoDetalle;
            AdminPlanPagoInterfaceClient AdminPlanPago = new AdminPlanPagoInterfaceClient();

            if (etAdminPlanPagoDetalle.Mensualidad == null || etAdminPlanPagoDetalle.Mensualidad == string.Empty)
            {
                NumError = 1;
                MensajeError = "Debe ingresar una Mensualidad valida.";
            }

            if (AdminPlanPago.WebAdminPlanPagoDetalleExisteMensualidad((string)Session["SesionSubCompania"], etAdminPlanPagoDetalle.Grupo, etAdminPlanPagoDetalle.Linea, etAdminPlanPagoDetalle.Mensualidad, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]))
            {
                NumError = 1;
                MensajeError += " La Mensualidad que esta tratando de Adicionar ya esta siendo usada.";
            }

            if (NumError == 0)
            {
                //editamos el detalle
                AdminPlanPagoSet setAdminPlanPago = AdminPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], etAdminPlanPagoDetalle.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etAdminPlanPagoDetalleEdit = setAdminPlanPago.ltAdminPlanPagoDetalle.Where(x => x.Grupo == etAdminPlanPagoDetalle.Grupo && x.Linea == etAdminPlanPagoDetalle.Linea).First();

                etAdminPlanPagoDetalleEdit.FechaLimite = etAdminPlanPagoDetalle.FechaLimite;
                etAdminPlanPagoDetalleEdit.Mensualidad = etAdminPlanPagoDetalle.Mensualidad;
                etAdminPlanPagoDetalleEdit.MontoSugerido = etAdminPlanPagoDetalle.MontoSugerido;
                etAdminPlanPagoDetalleEdit.UEstado = AdminPlanPagoService.Estado.Modified;
                //setAdminPlanPago.ltAdminPlanPagoDetalle.Add(etAdminPlanPagoDetalle);

                NumError = AdminPlanPago.WebGuardar(setAdminPlanPago, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminPlanPagoEditar", "AdminPlanPago", new { Grupo = etAdminPlanPagoDetalle.Grupo }) });
            }
            else
            {
                AdminCatalogoMensualidadInterfaceClient AdminCatalogoMensualidad = new AdminCatalogoMensualidadInterfaceClient();
                List<AdminCatalogoMensualidadEntity> ltAdminCatalogoMensualidad = AdminCatalogoMensualidad.WebAdminCatalogoMensualidadSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltAdminCatalogoMensualidad = ltAdminCatalogoMensualidad.Select(s => s.Mensualidad).ToList();

                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminPlanPagoDetalleOriginal);
            }

        }        
    }
}