using adminlte.TASAsistenciaEstudianteService;
using adminlte.TASGrupoService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASAsistenciaEstudianteController : BaseController
    {
        // GET: TASAsistenciaEstudiante
        public ActionResult TASAsistenciaEstudiantePDF()
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            var ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo);

            return View();
        }
        public ActionResult TASAsistenciaEstudiantePDFLista(string Grupo)
        {
            TASAsistenciaEstudianteInterfaceClient TASAsistenciaEstudiante = new TASAsistenciaEstudianteInterfaceClient();
            List<TASAsistenciaEstudianteEntity> ltTASAsistenciaEstudiante = TASAsistenciaEstudiante.WebTASAsistenciaEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASAsistenciaEstudiante = ltTASAsistenciaEstudiante.Where(x => x.Grupo == Grupo).ToList();

            return View(ltTASAsistenciaEstudiante);
        }

        public ActionResult TASAsistenciaEstudianteDescargarPDF(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria)
        {
            TASAsistenciaEstudianteInterfaceClient TASAsistenciaEstudiante = new TASAsistenciaEstudianteInterfaceClient();
            TASAsistenciaEstudianteSet setTASAsistenciaEstudiante = TASAsistenciaEstudiante.WebSeleccionar(SubCompania, Programa, Grupo, Materia, ModuloMateria, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            var ltTASAsistenciaEstudianteFechaLinea = setTASAsistenciaEstudiante.ltTASAsistenciaEstudianteFechaLinea;

            //Mostramos en el reporte
            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            MapPath = "~/Reports/TASAsistenciaEstudiante/TASAsistenciaEstudianteReporte.rpt";
            FileDownloadName = "TASAsistenciaEstudiante_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";

            try
            {
                //MapPath = "~/Reports/CDComanda/CDTTransaccion.rpt";
                //FileDownloadName = "CDTTransaccion" + DateTime.Now + ".xls";

                ReportDocument report = new ReportDocument();
                report.Load(Server.MapPath(MapPath));
                report.SetDataSource(ltTASAsistenciaEstudianteFechaLinea);
                //report.SetDataSource();
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                //Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                Stream stream = report.ExportToStream(ExportFormat);
                stream.Seek(0, SeekOrigin.Begin);
                //GetDowndoaldFile(stream);

                if (NumError == 0)
                {
                    return File(stream, ContentType, FileDownloadName);
                }
                else
                {
                    return RedirectToAction("Index", "Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Index");
            }
        }
    }
}