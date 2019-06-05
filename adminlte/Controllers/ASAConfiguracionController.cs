using adminlte.ASAConfiguracionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class ASAConfiguracionController : BaseController
    {
        // GET: ASAConfiguracion
        public ActionResult ASAConfiguracionEditar()
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionSet setASAConfiguracion = new ASAConfiguracionSet();
            List<ASAConfiguracionEntity> ltASAConfiguracion = ASAConfiguracion.WebASAConfiguracionSeleccionarXSubCompania((string)Session["SesionSubCompania"],(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (ltASAConfiguracion.Count > 0)
            {
                //si es mayor a 0 significa que existe registr y es para editar
                setASAConfiguracion = ASAConfiguracion.WebSeleccionar((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }
            else
            {
                //no existe registro y se considera uno nuevo
                setASAConfiguracion = ASAConfiguracion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            return View(setASAConfiguracion);
        }
        [HttpPost]
        public ActionResult ASAConfiguracionEditar(ASAConfiguracionSet setASAConfiguracion, DateTime? FechaInicial)
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionEntity etASAConfiguracion = setASAConfiguracion.ltASAConfiguracion.First();
            DateTime FechaIni = FechaInicial.Value;

            List<ASAConfiguracionEntity> ltASAConfiguracion = ASAConfiguracion.WebASAConfiguracionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (ltASAConfiguracion.Count > 0)
            {
                ASAConfiguracionSet setASAConfiguracionAntiguo = ASAConfiguracion.WebSeleccionar((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ASAConfiguracionEntity etASAConfiguracionAntiguo = setASAConfiguracionAntiguo.ltASAConfiguracion.First();
                etASAConfiguracionAntiguo.SubCompania = (string)Session["SesionSubCompania"];
                etASAConfiguracionAntiguo.CantidadPregunta = etASAConfiguracion.CantidadPregunta == null ? 100 : etASAConfiguracion.CantidadPregunta;
                etASAConfiguracionAntiguo.FechaIni = FechaIni == null ? DateTime.Now : FechaIni.Date + etASAConfiguracion.FechaIni.TimeOfDay;
                etASAConfiguracionAntiguo.FechaFin = FechaIni == null ? DateTime.Now.AddHours(1) : FechaIni.Date + etASAConfiguracion.FechaFin.TimeOfDay;
                etASAConfiguracionAntiguo.UEstado = Estado.Modified;
                long NumError = ASAConfiguracion.WebGuardar(setASAConfiguracionAntiguo,false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            }
            else
            {
                ASAConfiguracionSet setASAConfiguracionNuevo = ASAConfiguracion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ASAConfiguracionEntity etASAConfiguracionNuevo = setASAConfiguracionNuevo.ltASAConfiguracion.First();
                etASAConfiguracionNuevo.SubCompania = (string)Session["SesionSubCompania"];
                etASAConfiguracionNuevo.CantidadPregunta = etASAConfiguracion.CantidadPregunta == null ? 100 : etASAConfiguracion.CantidadPregunta;
                etASAConfiguracionNuevo.FechaIni = FechaIni == null ? DateTime.Now : FechaIni.Date + etASAConfiguracion.FechaIni.TimeOfDay;
                etASAConfiguracionNuevo.FechaFin = FechaIni == null ? DateTime.Now.AddHours(1) : FechaIni.Date + etASAConfiguracion.FechaFin.TimeOfDay;
                etASAConfiguracionNuevo.UEstado = Estado.Added;
                long NumError = ASAConfiguracion.WebGuardar(setASAConfiguracionNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            }

            return RedirectToAction("Index", "Index");
        }

    }
}