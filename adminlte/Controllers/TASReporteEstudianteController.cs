using adminlte.TASEstudianteMateriasAsignacionService;
using adminlte.TASRegistroNotasService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASReporteEstudianteController : BaseController
    {
        // GET: TASReporteEstudiante
        public ActionResult TASReporteMateriasFaltantes()
        {
            TASEstudianteMateriasAsignacionInterfaceClient TASEstudianteMateriasAsignacion = new TASEstudianteMateriasAsignacionInterfaceClient();
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();

            //Seleccionamos las materias asignadas
            List<TASEstudianteMateriasAsignacionEntity> ltTASEstudianteMateriasAsignacion = TASEstudianteMateriasAsignacion.WebTASEstudianteMateriasAsignacionSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASEstudianteMateriasAsignacion = ltTASEstudianteMateriasAsignacion.Where(x => x.EstudianteCI == (string)Session["Usuario"]).ToList();

            //Seleccionamos las notas de este alumno
            List<TASRegistroNotasEstudianteEntity> ltRegistroNotasEstudiante = TASRegistroNotas.WebTASRegistroNotasEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltRegistroNotasEstudiante = ltRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == (string)Session["Usuario"]).ToList();

            //Marcamos las notas que estan y no estan terminadas
            foreach (TASEstudianteMateriasAsignacionEntity etTASEstudianteMateriasAsignacion in ltTASEstudianteMateriasAsignacion)
            {
                if (ltRegistroNotasEstudiante.Any(any => any.Programa == etTASEstudianteMateriasAsignacion.Programa && any.Grupo == etTASEstudianteMateriasAsignacion.Grupo && any.Materia == etTASEstudianteMateriasAsignacion.Materia && any.ModuloMateria == etTASEstudianteMateriasAsignacion.ModuloMateria && any.Gestion == etTASEstudianteMateriasAsignacion.Gestion))
                {
                    //significa que ya fue calificado
                    etTASEstudianteMateriasAsignacion.Logico1 = true;
                }
                else
                {
                    etTASEstudianteMateriasAsignacion.Logico1 = false;
                }
            }

            //Mostramos en el reporte

            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            MapPath = "~/Reports/TASReporteEstudiante/TASReporteMateriasFaltantes.rpt";
            FileDownloadName = "TASReporteMateriasFaltantes_"+ (string)Session["Usuario"]+ "_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";
            
            try
            {
                //MapPath = "~/Reports/CDComanda/CDTTransaccion.rpt";
                //FileDownloadName = "CDTTransaccion" + DateTime.Now + ".xls";

                ReportDocument report = new ReportDocument();
                report.Load(Server.MapPath(MapPath));
                report.SetDataSource(ltTASEstudianteMateriasAsignacion);
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
                    return RedirectToAction("Index","Index");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Index");
            }

        }
    }
}