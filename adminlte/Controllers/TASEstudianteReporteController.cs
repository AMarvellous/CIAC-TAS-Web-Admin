using adminlte.Classes;
using adminlte.TASEstudianteService;
using adminlte.TASGestionService;
using adminlte.TASGrupoService;
using adminlte.TASProgramaService;
using adminlte.TASRegistroNotasService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASEstudianteReporteController : BaseController
    {
        // GET: TASEstudianteReporte
        public ActionResult TASEstudianteListaActual(string MensajeError = "")
        {
            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            var ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASPrograma = ltTASPrograma.Select(s => s.Programa).ToList();

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            var ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            //TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();
            //ViewBag.ltTASGestion = TASGestion.WebTASGestionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            //List<SelectListItem> ltMeses = new List<SelectListItem>() {
            //    new SelectListItem() { Value="0", Text="Seleccione un Mes" },
            //    new SelectListItem() { Value="1", Text="Enero" },
            //    new SelectListItem() { Value="2", Text="Febrero" },
            //    new SelectListItem() { Value="3", Text="Marzo" },
            //    new SelectListItem() { Value="4", Text="Abril" },
            //    new SelectListItem() { Value="5", Text="Mayo" },
            //    new SelectListItem() { Value="6", Text="Junio" },
            //    new SelectListItem() { Value="7", Text="Julio" },
            //    new SelectListItem() { Value="8", Text="Agosto" },
            //    new SelectListItem() { Value="9", Text="Septiembre" },
            //    new SelectListItem() { Value="10", Text="Octubre" },
            //    new SelectListItem() { Value="11", Text="Noviembre" },
            //    new SelectListItem() { Value="12", Text="Diciembre" },
            //};

            ViewBag.MensajeError = MensajeError;

            return View();
        }

        [HttpPost]
        public ActionResult TASEstudianteListaActual(string Grupo, string Programa)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;

            MapPath = "~/Reports/TASEstudianteLista/TASEstudianteListaReporte.rpt";
            FileDownloadName = "TASEstudianteListaActual_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";

            

            if (Grupo != "" && Programa != "" )
            {   
                try
                {
                    TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
                    List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo && x.Programa == Programa && x.Texto0 != "BajaEstudiante").ToList();

                    ReportDocument report = new ReportDocument();
                    

                    report.Load(Server.MapPath(MapPath));
                    report.SetDataSource(ltTASEstudiante);
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
                        return RedirectToAction("TASEstudianteListaActual", new { MensajeError = "Error al generar el reporte." });
                    }



                }
                catch (Exception e)
                {
                    return RedirectToAction("TASEstudianteListaActual", new { MensajeError = e.Message });
                }

            }
            else
            {
                MensajeError = "Debe seleccionar un programa y grupo.";
                return RedirectToAction("TASEstudianteListaActual", new { MensajeError = MensajeError });
            }
        }

        public ActionResult TASEstudianteCertificadoCalificaciones(string MensajeError = "", long Aux=0)
        {

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            List<SelectListItem> Estudiante = new List<SelectListItem>() {
                new SelectListItem() { Value="0", Text="Seleccione un Estudiante" },
           };

            ViewBag.Estudiante = Estudiante;

            //ViewBag.ltAXFUsuarioClaseUsuario = ltUsuarios;
            ViewBag.MensajeError = MensajeError;

            return View();
        }
        [HttpPost]
        public ActionResult TASEstudianteCertificadoCalificaciones(string Estudiante)
        {

            long NumError = 0;
            string MensajeError = string.Empty;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;

            MapPath = "~/Reports/TASEstudianteCalificaciones/TASEstudianteCalificaionesReporte.rpt";
            FileDownloadName = "TASEstudianteCalificaciones_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";



            if (Estudiante != "" )
            {
                try
                {
                    NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
                    numberFormatWithComma.NumberDecimalSeparator = ".";

                    List<TASCertificadoCalificacionesClass> TASCertificadoCalificaciones = new List<TASCertificadoCalificacionesClass>();
                    TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
                    List< TASRegistroNotasEstudianteEntity> ltTASRegistroNotasEstudiante = TASRegistroNotas.WebTASRegistroNotasEstudianteSeleccionarXSubCompaniaEstudiante((string)Session["SesionSubCompania"], Estudiante ,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    foreach (var NotasEstudiante in ltTASRegistroNotasEstudiante)
                    {
                        var TASRegistroNotasSet = TASRegistroNotas.WebSeleccionar(NotasEstudiante.SubCompania, NotasEstudiante.Programa, NotasEstudiante.Grupo, NotasEstudiante.Materia, NotasEstudiante.ModuloMateria, NotasEstudiante.Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        TASRegistroNotasEntity etTASRegistroNotas = TASRegistroNotasSet.ltTASRegistroNotas.First();
                        TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudiante = TASRegistroNotasSet.ltTASRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == Estudiante).First();

                        List<string> ltNotaIndividualProgreso = etTASRegistroNotasEstudiante.Progreso1P.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
                        List<string> ltPorcentajeIndividualProgreso = etTASRegistroNotas.PorcentajeIndividualProgreso.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
                        decimal PorcentajeTotalDominio1 = etTASRegistroNotas.PorcentajeDominio1D;
                        decimal Dominio1DNota = decimal.Parse(etTASRegistroNotasEstudiante.Dominio1D, numberFormatWithComma);


                        TASCertificadoCalificaciones.Add(new TASCertificadoCalificacionesClass
                        {
                            Nombre = etTASRegistroNotasEstudiante.NombreEstudiante,
                            Programa = etTASRegistroNotasEstudiante.Programa,
                            CI = etTASRegistroNotasEstudiante.CodigoTASEstudiante,
                            CodigoTAS = etTASRegistroNotasEstudiante.CodigoTASEstudiante,
                            Gestion = etTASRegistroNotasEstudiante.Gestion,
                            Materia = etTASRegistroNotasEstudiante.Materia,
                            ModuloMateria = etTASRegistroNotasEstudiante.ModuloMateria,
                            Numeral = CalcularNotaFinal(ltNotaIndividualProgreso, ltPorcentajeIndividualProgreso, PorcentajeTotalDominio1, Dominio1DNota).ToString(),
                            Observaciones = "",
                            Literal = ""
                        });
                    }


                    ReportDocument report = new ReportDocument();


                    report.Load(Server.MapPath(MapPath));
                    report.SetDataSource(TASCertificadoCalificaciones);
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
                        return RedirectToAction("TASEstudianteCertificadoCalificaciones", new { MensajeError = "Error al generar el reporte." });
                    }



                }
                catch (Exception e)
                {
                    return RedirectToAction("TASEstudianteCertificadoCalificaciones", new { MensajeError = e.Message });
                }

            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("TASEstudianteCertificadoCalificaciones", new { MensajeError = MensajeError });
            }
            
        }

        public JsonResult ObtenerEstudiantesGrupo(string Grupo)
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo).ToList();

            return Json(new { success = true, ltTASEstudiante = ltTASEstudiante }, JsonRequestBehavior.AllowGet);
        }

        private decimal CalcularNotaFinal(List<string> ltNotaIndividualProgreso, List<string> ltPorcentajeIndividualProgreso, decimal PorcentajeTotalDominio1, decimal Dominio1DNota)
        {
            decimal NotaFinal = 0;
            NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
            numberFormatWithComma.NumberDecimalSeparator = ".";

            try
            {
                //iteramos sobre las notas individuales
                for (int cont = 0; cont < ltPorcentajeIndividualProgreso.Count; cont++)
                {
                    decimal porcentajeNota = decimal.Parse(ltPorcentajeIndividualProgreso[cont], numberFormatWithComma);
                    decimal porcentajeCalculable = porcentajeNota / 100;
                    decimal notaIndividual = decimal.Parse(ltNotaIndividualProgreso[cont], numberFormatWithComma);
                    decimal NotaCalculada = porcentajeCalculable * notaIndividual;
                    NotaFinal += NotaCalculada;
                }

                decimal PorcentajeCalculable = PorcentajeTotalDominio1 / 100;
                decimal NotaDominioCalculada = Dominio1DNota * PorcentajeCalculable;
                NotaFinal += NotaDominioCalculada;
            }
            catch (Exception e)
            {
                NotaFinal = 0;
            }

            return NotaFinal;
        }
        
    }
}