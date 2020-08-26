using adminlte.AdminReglasPlanPagoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AdminReglasPlanPagoController : BaseController
    {
        // GET: AdminReglasPlanPago
        public ActionResult AdminReglasPlanPagoLista()
        {
            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();
            var ltAdminReglasPlanPago = AdminReglasPlanPago.WebAdminReglasPlanPagoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltAdminReglasPlanPago);
        }

        [HttpGet]
        public ActionResult AdminReglasPlanPagoNuevo(string MensajeError = "")
        {
            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();
            AdminReglasPlanPagoSet setAdminReglasPlanPago = AdminReglasPlanPago.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminReglasPlanPago);
        }

        [HttpPost]
        public ActionResult AdminReglasPlanPagoNuevo(AdminReglasPlanPagoSet setAdminReglasPlanPago)
        {
            long NumError = 0;
            long Linea = 0;
            string MensajeError = string.Empty;

            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();

            if (setAdminReglasPlanPago.ltAdminReglasPlanPago != null)
            {
                AdminReglasPlanPagoEntity etAdminReglasPlanPago = setAdminReglasPlanPago.ltAdminReglasPlanPago.First();
                if (etAdminReglasPlanPago.NombreRegla == null || etAdminReglasPlanPago.NombreRegla == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un nombre. ";
                }

                if (etAdminReglasPlanPago.DiaIni >= etAdminReglasPlanPago.DiaFin)
                {
                    NumError = 1;
                    MensajeError += "El dia de inicio no puede ser mayor al dia final. ";
                }

                if (etAdminReglasPlanPago.DiaIni < 0 || etAdminReglasPlanPago.DiaFin < 0)
                {
                    NumError = 1;
                    MensajeError += "El dia de inicio y fin no pueden ser negativos. ";
                }

                if (NumError == 0)
                {
                    AdminReglasPlanPagoSet setAdminReglasPlanPagoNuevo = AdminReglasPlanPago.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    AdminReglasPlanPagoEntity etAdminReglasPlanPagoNuevo = setAdminReglasPlanPagoNuevo.ltAdminReglasPlanPago.First();
                    etAdminReglasPlanPagoNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    long tick = DateTime.Now.Ticks;
                    etAdminReglasPlanPagoNuevo.Linea = tick;

                    etAdminReglasPlanPagoNuevo.NombreRegla = etAdminReglasPlanPago.NombreRegla == null ? "" : etAdminReglasPlanPago.NombreRegla;
                    etAdminReglasPlanPagoNuevo.DiaIni = etAdminReglasPlanPago.DiaIni;
                    etAdminReglasPlanPagoNuevo.DiaFin = etAdminReglasPlanPago.DiaFin;
                    etAdminReglasPlanPagoNuevo.MontoMulta = etAdminReglasPlanPago.MontoMulta;

                    etAdminReglasPlanPagoNuevo.UEstado = AdminReglasPlanPagoService.Estado.Added;
                    Linea = tick;
                    NumError = AdminReglasPlanPago.WebGuardar(setAdminReglasPlanPagoNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("AdminReglasPlanPagoEditar", new { SubCompania = (string)Session["SesionSubCompania"], Linea = Linea });
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setAdminReglasPlanPago);
            }

        }

        [HttpGet]
        public ActionResult AdminReglasPlanPagoEditar(string SubCompania, long Linea, string MensajeError = "")
        {
            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();
            AdminReglasPlanPagoSet setAdminReglasPlanPago = new AdminReglasPlanPagoSet();
            setAdminReglasPlanPago = AdminReglasPlanPago.WebSeleccionar(SubCompania, Linea, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.MensajeError = MensajeError;

            return View(setAdminReglasPlanPago);
        }

        [HttpPost]
        public ActionResult AdminReglasPlanPagoEditar(AdminReglasPlanPagoSet setAdminReglasPlanPago)
        {
            long NumError = 0;
            long Linea = 0;
            string MensajeError = string.Empty;

            AdminReglasPlanPagoInterfaceClient AdminReglasPlanPago = new AdminReglasPlanPagoInterfaceClient();

            if (setAdminReglasPlanPago.ltAdminReglasPlanPago != null)
            {
                AdminReglasPlanPagoEntity etAdminReglasPlanPago = setAdminReglasPlanPago.ltAdminReglasPlanPago.First();
                if (etAdminReglasPlanPago.NombreRegla == null || etAdminReglasPlanPago.NombreRegla == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un nombre. ";
                }

                if (etAdminReglasPlanPago.DiaIni >= etAdminReglasPlanPago.DiaFin)
                {
                    NumError = 1;
                    MensajeError += "El dia de inicio no puede ser mayor al dia final. ";
                }

                if (etAdminReglasPlanPago.DiaIni < 0 || etAdminReglasPlanPago.DiaFin < 0)
                {
                    NumError = 1;
                    MensajeError += "El dia de inicio y fin no pueden ser negativos. ";
                }

                if (NumError == 0)
                {
                    AdminReglasPlanPagoSet setAdminReglasPlanPagoEdit = AdminReglasPlanPago.WebSeleccionar((string)Session["SesionSubCompania"], etAdminReglasPlanPago.Linea, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    AdminReglasPlanPagoEntity etAdminReglasPlanPagoEdit = setAdminReglasPlanPagoEdit.ltAdminReglasPlanPago.First();
                    //CodigoTAS Same as EstudianteCI
                    etAdminReglasPlanPagoEdit.NombreRegla = etAdminReglasPlanPago.NombreRegla == null ? "" : etAdminReglasPlanPago.NombreRegla;
                    etAdminReglasPlanPagoEdit.DiaIni = etAdminReglasPlanPago.DiaIni;
                    etAdminReglasPlanPagoEdit.DiaFin = etAdminReglasPlanPago.DiaFin;
                    etAdminReglasPlanPagoEdit.MontoMulta = etAdminReglasPlanPago.MontoMulta;
                    etAdminReglasPlanPagoEdit.UEstado = AdminReglasPlanPagoService.Estado.Modified;

                    Linea = etAdminReglasPlanPago.Linea;
                    NumError = AdminReglasPlanPago.WebGuardar(setAdminReglasPlanPagoEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("AdminReglasPlanPagoLista");
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return View(setAdminReglasPlanPago);
            }
        }
    }
}