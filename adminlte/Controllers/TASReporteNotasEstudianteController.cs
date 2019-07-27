using adminlte.TASMaterialProgramaAnaliticoService;
using adminlte.TASRegistroNotasService;
using adminlte.TASReporteNotasEstudianteService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASReporteNotasEstudianteController : BaseController
    {
        // GET: TASReporteNotasEstudiante
        public ActionResult TASReporteListaEstudiante()
        {
            //reporte de los estudiantes que actualmente su programa analitico este abierto (Osea q no se cerro su materia)
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            List<TASMaterialProgramaAnaliticoEntity> ltTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebTASMaterialProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASMaterialProgramaAnalitico = ltTASMaterialProgramaAnalitico.Where(x => x.InstructorNroLicencia == (string)Session["Usuario"]).ToList();

            return View(ltTASMaterialProgramaAnalitico);
        }
        public ActionResult TASReporteListaEstudianteReporteGrupo(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            MapPath = "~/Reports/TASProgramaAnalitico/TASProgramaAnaliticoInstructor.rpt";
            FileDownloadName = "TASProgramaAnalitico_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            List<TASReporteNotasEstudianteEntity> ltTASReporteNotasEstudiante = new List<TASReporteNotasEstudianteEntity>();

            try
            {
                //MapPath = "~/Reports/CDComanda/CDTTransaccion.rpt";
                //FileDownloadName = "CDTTransaccion" + DateTime.Now + ".xls";

                ReportDocument report = new ReportDocument();

                TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar(SubCompania,Programa,Grupo,Materia,ModuloMateria,Gestion,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                TASRegistroNotasEntity etTASRegistroNotas = setTASRegistroNotas.ltTASRegistroNotas.First();
                foreach (var item in setTASRegistroNotas.ltTASRegistroNotasEstudiante)
                {
                    ltTASReporteNotasEstudiante.Add(new TASReporteNotasEstudianteEntity {
                        SubCompania = item.SubCompania,
                        Programa = item.Programa,
                        Grupo = item.Grupo,
                        Materia = item.Materia,
                        ModuloMateria = item.ModuloMateria,
                        Gestion = item.Gestion,
                        CodigoTASEstudiante = item.CodigoTASEstudiante,
                        InstructorLic = etTASRegistroNotas.InstructorLic,

                    });
                }


                report.Load(Server.MapPath(MapPath));
                report.SetDataSource(ltTASReporteNotasEstudiante);
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
                    return RedirectToAction("TASProgramaAnaliticoGestionar");
                }



            }
            catch (Exception e)
            {
                return RedirectToAction("TASProgramaAnaliticoGestionar");
            }
            return View();
        }

    }
}