using adminlte.TASProgramacionHorarioInstructorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASProgramacionHorarioInstructorController : BaseController
    {
        // GET: TASProgramacionHorarioInstructor
        public ActionResult TASProgramacionHorarioInstructorLista()
        {
            TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
            List<TASProgramacionHorarioInstructorLineaEntity> ltTASProgramacionHorarioInstructorLinea = TASProgramacionHorarioInstructor.WebTASProgramacionHorarioInstructorLineaSeleccionarKeyTASProgramacionHorarioInstructor((string)Session["SesionSubCompania"], (string)Session["Usuario"],(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            
            return View(ltTASProgramacionHorarioInstructorLinea);
        }

        public JsonResult GetEvents()
        {
            TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
            List<TASProgramacionHorarioInstructorLineaEntity> ltTASProgramacionHorarioInstructorLinea = TASProgramacionHorarioInstructor.WebTASProgramacionHorarioInstructorLineaSeleccionarKeyTASProgramacionHorarioInstructor((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return Json(new { success = true, events = ltTASProgramacionHorarioInstructorLinea }, JsonRequestBehavior.AllowGet);
        }
    }
}