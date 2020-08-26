using adminlte.AdminAsignacionReglasPlanPagoService;
using adminlte.AdminReglasPlanPagoService;
using adminlte.TASGrupoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace adminlte.Controllers
{
    public class AdminAsignacionReglasPlanPagoController : BaseController
    {
        // GET: AdminAsignacionReglasPlanPago
        public ActionResult AdminAsignacionReglasPlanPagoLista()
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            var ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASGrupo);
        }

        [HttpGet]
        public ActionResult AdminReglasPlanPagoEditar(string SubCompania, string Grupo, string MensajeError = "")
        {
            AdminAsignacionReglasPlanPagoInterfaceClient AdminAsignacionReglasPlanPago = new AdminAsignacionReglasPlanPagoInterfaceClient();
            List<AdminAsignacionReglasPlanPagoEntity> ltSetAdminAsignacionReglasPlanPago = AdminAsignacionReglasPlanPago.WebSeleccionarXGrupoKey(SubCompania, Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.MensajeError = MensajeError;
            ViewBag.Grupo = Grupo;
            
            return View(ltSetAdminAsignacionReglasPlanPago);
        }

        [HttpGet]
        public ActionResult AdminReglasPlanPagoAsignacion(string Grupo, string MensajeError = "")
        {
            AdminAsignacionReglasPlanPagoEntity etAdminAsignacionReglasPlanPago = new AdminAsignacionReglasPlanPagoEntity();
            etAdminAsignacionReglasPlanPago.SubCompania = (string)Session["SesionSubCompania"];
            etAdminAsignacionReglasPlanPago.Grupo = Grupo;

            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();
            List<AdminReglasPlanPagoEntity> ltAdminReglasPlanPago = AdminReglasPlanPago.WebAdminReglasPlanPagoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            var ltItem = new List<ListItem>();
            foreach (var etAdminReglasPlanPago in ltAdminReglasPlanPago)
            {
                ltItem.Add(new ListItem { Text = etAdminReglasPlanPago.NombreRegla, Value = etAdminReglasPlanPago.Linea.ToString()});
            } 

            ViewBag.ltAdminReglasPlanPago = new SelectList(ltItem, "Value", "Text");

            ViewBag.MensajeError = MensajeError;

            return PartialView(etAdminAsignacionReglasPlanPago);
        }

        [HttpPost]
        public ActionResult AdminReglasPlanPagoAsignacion(AdminAsignacionReglasPlanPagoEntity etAdminAsignacionReglasPlanPagoForm)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            AdminAsignacionReglasPlanPagoEntity etAdminAsignacionReglasPlanPagoOriginal = etAdminAsignacionReglasPlanPagoForm;
            AdminAsignacionReglasPlanPagoInterfaceClient AdminAsignacionReglasPlanPago = new AdminAsignacionReglasPlanPagoInterfaceClient();

            if (etAdminAsignacionReglasPlanPagoForm.LineaRegla == 0)
            {
                NumError = 1;
                MensajeError = "Debe ingresar una Regla valida.";
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                AdminAsignacionReglasPlanPagoSet setAdminAsignacionReglasPlanPago = AdminAsignacionReglasPlanPago.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etAdminAsignacionReglasPlanPago = setAdminAsignacionReglasPlanPago.ltAdminAsignacionReglasPlanPago.First();
                etAdminAsignacionReglasPlanPago.SubCompania = (string)Session["SesionSubCompania"];
                etAdminAsignacionReglasPlanPago.Grupo = etAdminAsignacionReglasPlanPagoForm.Grupo;
                etAdminAsignacionReglasPlanPago.LineaRegla = etAdminAsignacionReglasPlanPagoForm.LineaRegla;

                etAdminAsignacionReglasPlanPago.Texto0 = GetNombreReglaPlanDePago(etAdminAsignacionReglasPlanPago.SubCompania, etAdminAsignacionReglasPlanPago.LineaRegla);
                etAdminAsignacionReglasPlanPago.UEstado = AdminAsignacionReglasPlanPagoService.Estado.Added;

                NumError = AdminAsignacionReglasPlanPago.WebGuardar(setAdminAsignacionReglasPlanPago, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("AdminReglasPlanPagoEditar", "AdminAsignacionReglasPlanPago", new { SubCompania = (string)Session["SesionSubCompania"], Grupo = etAdminAsignacionReglasPlanPagoForm.Grupo }) });
            }
            else
            {
                AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();
                List<AdminReglasPlanPagoEntity> ltAdminReglasPlanPago = AdminReglasPlanPago.WebAdminReglasPlanPagoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var ltItem = new List<ListItem>();
                foreach (var etAdminReglasPlanPago in ltAdminReglasPlanPago)
                {
                    ltItem.Add(new ListItem { Text = etAdminReglasPlanPago.NombreRegla, Value = etAdminReglasPlanPago.Linea.ToString() });
                }

                ViewBag.ltAdminReglasPlanPago = new SelectList(ltItem, "Value", "Text");
                ViewBag.MensajeError = MensajeError;
                return PartialView(etAdminAsignacionReglasPlanPagoOriginal);
            }
        }

        private string GetNombreReglaPlanDePago(string SubCompania, long Linea)
        {
            string Nombre = string.Empty;
            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();
            var setAdminReglasPlanPago = AdminReglasPlanPago.WebSeleccionar(SubCompania, Linea, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            foreach (var item in setAdminReglasPlanPago.ltAdminReglasPlanPago)
            {
                Nombre = item.NombreRegla;
            }

            return Nombre;
        }
    }
}