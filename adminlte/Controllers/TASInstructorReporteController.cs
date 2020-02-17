using adminlte.Classes;
using adminlte.TASConfiguracionEncuestaSatisfaccionInstructorService;
using adminlte.TASCriterioEvaluacionAlumnoInstructorService;
using adminlte.TASEncuestaSatisfaccionInstructorService;
using adminlte.TASEstudianteService;
using adminlte.TASRegistroNotasService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace adminlte.Controllers
{
    public class TASInstructorReporteController : BaseController
    {
        // GET: TASInstructorReporte
        public ActionResult TASInstructorReporteXMateria()
        {
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            List<TASRegistroNotasEntity> ltTASRegistroNotas = TASRegistroNotas.WebTASRegistroNotasSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASRegistroNotas = ltTASRegistroNotas.Where(x => x.InstructorLic == (string)Session["Usuario"]).ToList();

            return View(ltTASRegistroNotas);
        }

        public ActionResult TASInstructorReporteXMateriaDescargar(string Programa, string Grupo , string Materia , string ModuloMateria , string Gestion)
        {
            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();

            MapPath = "~/Reports/TASInstructorReporte/TASInstructorReporteXMateria.rpt";
            FileDownloadName = "Reporte_" + Materia + "_" + Grupo + "_" + Gestion + "_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";


            try
            {
                NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
                numberFormatWithComma.NumberDecimalSeparator = ".";

                ReportDocument report = new ReportDocument();
                TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar((string)Session["SesionSubCompania"], Programa,Grupo,Materia,ModuloMateria,Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                TASRegistroNotasEntity etTASRegistroNotas = setTASRegistroNotas.ltTASRegistroNotas.First();

                List<TASRegistroNotasEstudianteEntity> ltTASRegistroNotasEstudiante = setTASRegistroNotas.ltTASRegistroNotasEstudiante;

                foreach (TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudiante in ltTASRegistroNotasEstudiante)
                {
                    List<string> ltNotaIndividualProgreso = etTASRegistroNotasEstudiante.Progreso1P.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
                    List<string> ltPorcentajeIndividualProgreso = etTASRegistroNotas.PorcentajeIndividualProgreso.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
                    decimal PorcentajeTotalDominio1 = etTASRegistroNotas.PorcentajeDominio1D;
                    decimal Dominio1DNota = decimal.Parse(etTASRegistroNotasEstudiante.Dominio1D, numberFormatWithComma);

                    etTASRegistroNotasEstudiante.CalificacionNF =  CalcularNotaFinal(ltNotaIndividualProgreso, ltPorcentajeIndividualProgreso, PorcentajeTotalDominio1, Dominio1DNota).ToString();
                    etTASRegistroNotasEstudiante.Progreso1P = CalcularNotaProgreso1(ltNotaIndividualProgreso, ltPorcentajeIndividualProgreso).ToString();
                    etTASRegistroNotasEstudiante.Dominio1D = CalcularDominio(PorcentajeTotalDominio1, Dominio1DNota).ToString();
                }

                report.Load(Server.MapPath(MapPath));
                report.SetDataSource(ltTASRegistroNotasEstudiante);
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
                    return RedirectToAction("TASInstructorReporteXMateria");
                }



            }
            catch (Exception e)
            {
                return RedirectToAction("TASInstructorReporteXMateria");
            }
            
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

        private decimal CalcularNotaProgreso1(List<string> ltNotaIndividualProgreso, List<string> ltPorcentajeIndividualProgreso)
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

            }
            catch (Exception e)
            {
                NotaFinal = 0;
            }

            return NotaFinal;
        }

        private decimal CalcularDominio(decimal PorcentajeTotalDominio1, decimal Dominio1DNota)
        {
            decimal NotaFinal = 0;
            NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
            numberFormatWithComma.NumberDecimalSeparator = ".";

            try
            {
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

        public ActionResult TASEstudianteEncuestaSatisfaccionInstructor()
        {
            TASConfiguracionEncuestaSatisfaccionInstructorInterfaceClient TASConfiguracionEncuestaSatisfaccionInstructor = new TASConfiguracionEncuestaSatisfaccionInstructorInterfaceClient();
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            TASEncuestaSatisfaccionInstructorInterfaceClient TASEncuestaSatisfaccionInstructor = new TASEncuestaSatisfaccionInstructorInterfaceClient();
            
            List<TASConfiguracionEncuestaSatisfaccionInstructorEntity> ltTASConfiguracionEncuestaSatisfaccionInstructor = TASConfiguracionEncuestaSatisfaccionInstructor.WebTASConfiguracionEncuestaSatisfaccionInstructorSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASConfiguracionEncuestaSatisfaccionInstructor = ltTASConfiguracionEncuestaSatisfaccionInstructor.OrderBy(o => o.Fecha).Where(x => x.InstructorLic == (string)Session["Usuario"]).ToList();

            List<TASConfiguracionEncuestaSatisfaccionInstructorEntity> ltEncuestasPendientes = new List<TASConfiguracionEncuestaSatisfaccionInstructorEntity>();
            //si existe configuraciones, entonces iterar y chequear si tiene alguna pendiente de hacer entre todas las que hay
            foreach (TASConfiguracionEncuestaSatisfaccionInstructorEntity etTASConfiguracionEncuestaSatisfaccionInstructor in ltTASConfiguracionEncuestaSatisfaccionInstructor)
            {
                //checkeamos si el estudiante realizo la encuesta
                TASEncuestaSatisfaccionInstructorSet setTASEncuestaSatisfaccionInstructor = TASEncuestaSatisfaccionInstructor.WebSeleccionar(etTASConfiguracionEncuestaSatisfaccionInstructor.SubCompania, etTASConfiguracionEncuestaSatisfaccionInstructor.InstructorLic, etTASConfiguracionEncuestaSatisfaccionInstructor.Fecha, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (setTASEncuestaSatisfaccionInstructor.ltTASEncuestaSatisfaccionInstructorLinea.Count > 0)
                {
                    //long RespuestasEncuestaEstudiante = setTASEncuestaSatisfaccionInstructor.ltTASEncuestaSatisfaccionInstructorLinea.Where(x => x.EstudianteLic == (string)Session["Usuario"]).Count();
                    //if (RespuestasEncuestaEstudiante == 0)
                    //{
                    //    ltEncuestasPendientes.Add(etTASConfiguracionEncuestaSatisfaccionInstructor);
                    //}
                }
                else
                {
                    ltEncuestasPendientes.Add(etTASConfiguracionEncuestaSatisfaccionInstructor);
                }

            }

            //si no existe mandar a una pagina de mensaje
            if (ltEncuestasPendientes.Count == 0)
            {
                return View("TASInstructorEncuestaSatisfaccionInstructorRealizado");
            }
            else //Caso contrario mandar a la pagina para hacer la encuesta anual
            {
                TASCriterioEvaluacionAlumnoInstructorInterfaceClient TASCriterioEvaluacionAlumnoInstructor = new TASCriterioEvaluacionAlumnoInstructorInterfaceClient();
                List<TASCriterioEvaluacionAlumnoInstructorEntity> ltTASCriterioEvaluacionAlumnoInstructor = TASCriterioEvaluacionAlumnoInstructor.WebTASCriterioEvaluacionAlumnoInstructorSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                //List<GradoSatisfaccionClass> ltGradoSatisfaccion = new List<GradoSatisfaccionClass>();

                //string[] arrayGradoSatisfaccion = new string[] { "Muy Satisfactorio", "Satisfactorio", "Insatisfactorio", "Muy Insatisfactorio" };

                var listItems = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Muy Satisfactorio" , Value = "4" },
                        new ListItem { Text = "Satisfactorio" , Value = "3" },
                        new ListItem { Text = "Insatisfactorio" , Value = "2" },
                        new ListItem { Text = "Muy Insatisfactorio" , Value = "1" },
                   }, "Value", "Text");

                ViewBag.ltGradoSatisfaccion = listItems;

                //for (int i= 0; i < arrayGradoSatisfaccion.Length; i++ )
                //{
                //    //ltGradoSatisfaccion.Add(new GradoSatisfaccionClass {
                //        Puntuacion = i,
                //        Descripcion = arrayGradoSatisfaccion[i]
                //    //});
                //}

                TASEncuestaSatisfaccionInstructorDetalle etTASEncuestaSatisfaccionInstructorDetalle = new TASEncuestaSatisfaccionInstructorDetalle
                {
                    etTASConfiguracionEncuestaSatisfaccionInstructor = ltEncuestasPendientes.FirstOrDefault(),
                    ltCriterioEvaluacion = ltTASCriterioEvaluacionAlumnoInstructor.OrderBy(o => o.Linea).Select(x => x.Descripcion).ToList(),
                    ltLinea = ltTASCriterioEvaluacionAlumnoInstructor.OrderBy(o => o.Linea).Select(x => x.Linea).ToList(),
                    ltGradoSatisfaccion = ltTASCriterioEvaluacionAlumnoInstructor.OrderBy(o => o.Linea).Select(x => x.Entero1).ToList(),
                    ObservacionSugerencias = ""
                };

                return View("TASInstructorEncuestaInstructorEncuestar", etTASEncuestaSatisfaccionInstructorDetalle);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TASEstudianteEncuestaSatisfaccionInstructor(TASEncuestaSatisfaccionInstructorDetalle etTASEncuestaSatisfaccionInstructorDetalle)
        {
            TASEncuestaSatisfaccionInstructorInterfaceClient TASEncuestaSatisfaccionInstructor = new TASEncuestaSatisfaccionInstructorInterfaceClient();
            List<TASEncuestaSatisfaccionInstructorLineaEntity> ltTASEncuestaSatisfaccionInstructorLinea = new List<TASEncuestaSatisfaccionInstructorLineaEntity>();

            for (int cont = 0; cont < etTASEncuestaSatisfaccionInstructorDetalle.ltGradoSatisfaccion.Count; cont++)
            {
                ltTASEncuestaSatisfaccionInstructorLinea.Add(new TASEncuestaSatisfaccionInstructorLineaEntity
                {
                    SubCompania = (string)Session["SesionSubCompania"],
                    InstructorLicencia = etTASEncuestaSatisfaccionInstructorDetalle.etTASConfiguracionEncuestaSatisfaccionInstructor.InstructorLic,
                    Fecha = etTASEncuestaSatisfaccionInstructorDetalle.etTASConfiguracionEncuestaSatisfaccionInstructor.Fecha,
                    Linea = etTASEncuestaSatisfaccionInstructorDetalle.ltLinea[cont],
                    CriterioEvaluacion = etTASEncuestaSatisfaccionInstructorDetalle.ltLinea[cont],
                    Puntuacion = etTASEncuestaSatisfaccionInstructorDetalle.ltGradoSatisfaccion[cont],
                    ObservacionSugerencia = etTASEncuestaSatisfaccionInstructorDetalle.ObservacionSugerencias == null ? "" : etTASEncuestaSatisfaccionInstructorDetalle.ObservacionSugerencias
                });
            }

            TASEncuestaSatisfaccionInstructorSet setTASEncuestaSatisfaccionInstructor = TASEncuestaSatisfaccionInstructor.WebSeleccionar((string)Session["SesionSubCompania"], etTASEncuestaSatisfaccionInstructorDetalle.etTASConfiguracionEncuestaSatisfaccionInstructor.InstructorLic, etTASEncuestaSatisfaccionInstructorDetalle.etTASConfiguracionEncuestaSatisfaccionInstructor.Fecha, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            setTASEncuestaSatisfaccionInstructor.ltTASEncuestaSatisfaccionInstructorLinea.AddRange(ltTASEncuestaSatisfaccionInstructorLinea);
            setTASEncuestaSatisfaccionInstructor.ltTASEncuestaSatisfaccionInstructor.First().UEstado = TASEncuestaSatisfaccionInstructorService.Estado.Modified;

            long NumError = TASEncuestaSatisfaccionInstructor.WebGuardar(setTASEncuestaSatisfaccionInstructor, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return RedirectToAction("TASEstudianteEncuestaSatisfaccionInstructor");
        }
    }
}