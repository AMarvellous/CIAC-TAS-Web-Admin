using adminlte.AXFUsuarioService;
using adminlte.Classes;
using adminlte.TASConfiguracionEncuestaAlumnoInstructorService;
using adminlte.TASConfiguracionEncuestaSatisfaccionEstudianteService;
using adminlte.TASCriterioEncuestaSatisfaccionEstudianteService;
using adminlte.TASCriterioEvaluacionAlumnoInstructorService;
using adminlte.TASEncuestaAlumnoInstructorService;
using adminlte.TASEncuestaSatisfaccionEstudianteService;
using adminlte.TASEstudianteService;
using adminlte.TASMaterialDidacticoService;
using adminlte.TASRegistroNotasService;
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
    public class TASEstudianteController : BaseController
    {
        // GET: TASEstudianteNotas
        [HttpGet]
        public ActionResult TASEstudianteNotas()
        {
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            string Estudiante = (string)Session["Usuario"];

            //Verificamos si el estudiante no esta bloqueado para ver notas por mensualidad
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFUsuarioSet setAXFUsuario = AXFUsuario.WebSeleccionar(Estudiante, (string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            var etAXFUsuarioPropiedad = setAXFUsuario.ltAXFUsuarioPropiedad.Where(x => x.Propiedad == "BloqueoNotas").FirstOrDefault();
            if (etAXFUsuarioPropiedad != null && etAXFUsuarioPropiedad.Valor == "Si")
            {
                return View("TASBloqueoEstudiante");
            }
            else
            {
                var ltTASRegistroNotasEstudiante = TASRegistroNotas.WebTASRegistroNotasEstudianteSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltTASRegistroNotasEstudiante = ltTASRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == Estudiante).ToList();

                foreach (var etTASRegistroNotasEstudiante in ltTASRegistroNotasEstudiante)
                {
                    etTASRegistroNotasEstudiante.NotaLiteral = TASRegistroNotasEstudianteCalcularNotaFinal(etTASRegistroNotasEstudiante.Programa, etTASRegistroNotasEstudiante.Grupo, etTASRegistroNotasEstudiante.Materia, etTASRegistroNotasEstudiante.ModuloMateria, etTASRegistroNotasEstudiante.Gestion, etTASRegistroNotasEstudiante.CodigoTASEstudiante).ToString("0.##");
                }


                return View(ltTASRegistroNotasEstudiante);
            }            
        }

        [HttpGet]
        public ActionResult TASEstudianteNotasGrilla(string Programa, string Grupo, string Materia, string ModuloMateria , string Gestion, string CodigoTASEstudiante)
        {
            NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
            numberFormatWithComma.NumberDecimalSeparator = ".";

            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //List<TASRegistroNotasEstudianteEntity> ltTASRegistroNotasEstudiante = TASRegistroNotas.WebTASRegistroNotasEstudianteSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, CodigoTASEstudiante, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudiante = setTASRegistroNotas.ltTASRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == CodigoTASEstudiante).FirstOrDefault();

            TASRegistroNotasEntity etTASRegistroNotas = setTASRegistroNotas.ltTASRegistroNotas.First();
            ViewBag.ltTASRegistroNotas = etTASRegistroNotas;

            //decimal PorcentajeTotalProgreso1 = etTASRegistroNotas.PorcentajeProgreso1P;
            List<string> ltNotaIndividualProgreso = etTASRegistroNotasEstudiante.Progreso1P.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
            List<string> ltPorcentajeIndividualProgreso = etTASRegistroNotas.PorcentajeIndividualProgreso.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
            decimal PorcentajeTotalDominio1 = etTASRegistroNotas.PorcentajeDominio1D;
            decimal Dominio1DNota = decimal.Parse(etTASRegistroNotasEstudiante.Dominio1D, numberFormatWithComma);

            ViewBag.NotaFinalCalculada = CalcularNotaFinal(ltNotaIndividualProgreso, ltPorcentajeIndividualProgreso, PorcentajeTotalDominio1, Dominio1DNota);

            return View(etTASRegistroNotasEstudiante);
        }

        public decimal TASRegistroNotasEstudianteCalcularNotaFinal(string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string CodigoTASEstudiante)
        {
            NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
            numberFormatWithComma.NumberDecimalSeparator = ".";

            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //List<TASRegistroNotasEstudianteEntity> ltTASRegistroNotasEstudiante = TASRegistroNotas.WebTASRegistroNotasEstudianteSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, CodigoTASEstudiante, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudiante = setTASRegistroNotas.ltTASRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == CodigoTASEstudiante).FirstOrDefault();

            TASRegistroNotasEntity etTASRegistroNotas = setTASRegistroNotas.ltTASRegistroNotas.First();

            //decimal PorcentajeTotalProgreso1 = etTASRegistroNotas.PorcentajeProgreso1P;
            List<string> ltNotaIndividualProgreso = etTASRegistroNotasEstudiante.Progreso1P.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
            List<string> ltPorcentajeIndividualProgreso = etTASRegistroNotas.PorcentajeIndividualProgreso.Split(new string[] { "*.-Split-.*" }, StringSplitOptions.None).ToList();
            decimal PorcentajeTotalDominio1 = etTASRegistroNotas.PorcentajeDominio1D;
            decimal Dominio1DNota = decimal.Parse(etTASRegistroNotasEstudiante.Dominio1D, numberFormatWithComma);

            return CalcularNotaFinal(ltNotaIndividualProgreso, ltPorcentajeIndividualProgreso, PorcentajeTotalDominio1, Dominio1DNota);
        }

        public decimal CalcularNotaFinal(List<string> ltNotaIndividualProgreso ,List<string> ltPorcentajeIndividualProgreso, decimal PorcentajeTotalDominio1, decimal Dominio1DNota)
        {
            decimal NotaFinal = 0;
            NumberFormatInfo numberFormatWithComma = new NumberFormatInfo();
            numberFormatWithComma.NumberDecimalSeparator = ".";

            try
            {
                //iteramos sobre las notas individuales
                for(int cont=0; cont < ltPorcentajeIndividualProgreso.Count; cont++)
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


        [HttpGet]
        public ActionResult TASEstudianteMaterialDidactico()
        {
            TASMaterialDidacticoInterfaceClient TASMaterialDidactico = new TASMaterialDidacticoInterfaceClient();
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionar((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASEstudianteEntity etTASEstudiante = ltTASEstudiante.FirstOrDefault();
            List<TASMaterialDidacticoEntity> ltTASMaterialDidactico = TASMaterialDidactico.WebTASMaterialDidacticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASMaterialDidactico = ltTASMaterialDidactico.Where(x => x.Programa == etTASEstudiante.Programa && x.Grupo == etTASEstudiante.Grupo).ToList();

            return View(ltTASMaterialDidactico);
        }

        public ActionResult TASMaterialDidacticoVisualizar(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion)
        {
            TASMaterialDidacticoInterfaceClient TASMaterialDidactico = new TASMaterialDidacticoInterfaceClient();
            List<TASMaterialDidacticoEntity> ltTASMaterialDidactico = TASMaterialDidactico.WebTASMaterialDidacticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialDidacticoEntity etTASMaterialDidactico = ltTASMaterialDidactico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion).FirstOrDefault();

            return View(etTASMaterialDidactico);
        }

        public ActionResult TASMaterialDidacticoDescargarArchivo(string Archivo)
        {
            long NumError = 0;

            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Archivo);
                string fileName = Path.GetFileName(Archivo);

                if (NumError == 0)
                {
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    return RedirectToAction("TASEstudianteMaterialDidactico");
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TASEstudianteMaterialDidactico");
            }
        }

        public ActionResult TASEstudianteEncuestaSatisfaccionEstudiante()
        {
            TASConfiguracionEncuestaSatisfaccionEstudianteInterfaceClient TASConfiguracionEncuestaSatisfaccionEstudiante = new TASConfiguracionEncuestaSatisfaccionEstudianteInterfaceClient();
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            TASEncuestaSatisfaccionEstudianteInterfaceClient TASEncuestaSatisfaccionEstudiante = new TASEncuestaSatisfaccionEstudianteInterfaceClient();

            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionar((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASEstudianteEntity etTASEstudiante = ltTASEstudiante.FirstOrDefault();

            List<TASConfiguracionEncuestaSatisfaccionEstudianteEntity> ltTASConfiguracionEncuestaSatisfaccionEstudiante = TASConfiguracionEncuestaSatisfaccionEstudiante.WebTASConfiguracionEncuestaSatisfaccionEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASConfiguracionEncuestaSatisfaccionEstudiante = ltTASConfiguracionEncuestaSatisfaccionEstudiante.OrderBy(o => o.Fecha).Where(x => etTASEstudiante.Programa == x.Programa && x.Grupo == etTASEstudiante.Grupo).ToList();

            List<TASConfiguracionEncuestaSatisfaccionEstudianteEntity> ltEncuestasPendientes = new List<TASConfiguracionEncuestaSatisfaccionEstudianteEntity>();
            //si existe configuraciones, entonces iterar y chequear si tiene alguna pendiente de hacer entre todas las que hay
            foreach (TASConfiguracionEncuestaSatisfaccionEstudianteEntity etTASConfiguracionEncuestaSatisfaccionEstudiante in ltTASConfiguracionEncuestaSatisfaccionEstudiante)
            {
                //checkeamos si el estudiante realizo la encuesta
                TASEncuestaSatisfaccionEstudianteSet setTASEncuestaSatisfaccionEstudiante = TASEncuestaSatisfaccionEstudiante.WebSeleccionar(etTASConfiguracionEncuestaSatisfaccionEstudiante.SubCompania, etTASConfiguracionEncuestaSatisfaccionEstudiante.Programa, etTASConfiguracionEncuestaSatisfaccionEstudiante.Grupo, etTASConfiguracionEncuestaSatisfaccionEstudiante.Fecha, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if(setTASEncuestaSatisfaccionEstudiante.ltTASEncuestaSatisfaccionEstudianteLinea.Count > 0)
                {
                    long RespuestasEncuestaEstudiante = setTASEncuestaSatisfaccionEstudiante.ltTASEncuestaSatisfaccionEstudianteLinea.Where(x => x.EstudianteLic == (string)Session["Usuario"]).Count();
                    if(RespuestasEncuestaEstudiante == 0)
                    {
                        ltEncuestasPendientes.Add(etTASConfiguracionEncuestaSatisfaccionEstudiante);
                    }
                }
                else
                {
                    ltEncuestasPendientes.Add(etTASConfiguracionEncuestaSatisfaccionEstudiante);
                }

            }

            //si no existe mandar a una pagina de mensaje
            if(ltEncuestasPendientes.Count == 0)
            {                
                return View("TASEstudianteEncuestaSatisfaccionEstudianteRealizado");
            }
            else //Caso contrario mandar a la pagina para hacer la encuesta anual
            {
                TASCriterioEncuestaSatisfaccionEstudianteInterfaceClient TASCriterioEncuestaSatisfaccionEstudiante = new TASCriterioEncuestaSatisfaccionEstudianteInterfaceClient();
                List<TASCriterioEncuestaSatisfaccionEstudianteEntity> ltTASCriterioEncuestaSatisfaccionEstudiante = TASCriterioEncuestaSatisfaccionEstudiante.WebTASCriterioEncuestaSatisfaccionEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                //List<GradoSatisfaccionClass> ltGradoSatisfaccion = new List<GradoSatisfaccionClass>();

                //string[] arrayGradoSatisfaccion = new string[] { "Muy Satisfactorio", "Satisfactorio", "Insatisfactorio", "Muy Insatisfactorio" };

                var listItems = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Muy Satisfactorio" , Value = "4" },
                        new ListItem { Text = "Satisfactorio" , Value = "3" },
                        new ListItem { Text = "Insatisfactorio" , Value = "2" },
                        new ListItem { Text = "Muy Insatisfactorio" , Value = "1" },
                        new ListItem { Text = "No Aplica" , Value = "0" },
                   }, "Value", "Text");

                ViewBag.ltGradoSatisfaccion = listItems;

                //for (int i= 0; i < arrayGradoSatisfaccion.Length; i++ )
                //{
                //    //ltGradoSatisfaccion.Add(new GradoSatisfaccionClass {
                //        Puntuacion = i,
                //        Descripcion = arrayGradoSatisfaccion[i]
                //    //});
                //}

                TASEncuestaSatisfaccionEstudianteDetalle etTASEncuestaSatisfaccionEstudianteDetalle = new TASEncuestaSatisfaccionEstudianteDetalle {
                    etTASConfiguracionEncuestaSatisfaccionEstudiante = ltEncuestasPendientes.FirstOrDefault(),
                    ltCriterioEvaluacion = ltTASCriterioEncuestaSatisfaccionEstudiante.OrderBy(o => o.Linea).Select(x => x.Descripcion).ToList(),
                    ltLinea = ltTASCriterioEncuestaSatisfaccionEstudiante.OrderBy(o => o.Linea).Select(x => x.Linea).ToList(),
                    ltGradoSatisfaccion = ltTASCriterioEncuestaSatisfaccionEstudiante.OrderBy(o => o.Linea).Select(x => x.Entero1).ToList(),
                    ObservacionSugerencias = ""
                };
                
                return View("TASEstudianteEncuestaSatisfaccionEstudianteEncuestar", etTASEncuestaSatisfaccionEstudianteDetalle);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TASEstudianteEncuestaSatisfaccionEstudiante(TASEncuestaSatisfaccionEstudianteDetalle etTASEncuestaSatisfaccionEstudianteDetalle)
        {
            TASEncuestaSatisfaccionEstudianteInterfaceClient TASEncuestaSatisfaccionEstudiante = new TASEncuestaSatisfaccionEstudianteInterfaceClient();
            List<TASEncuestaSatisfaccionEstudianteLineaEntity> ltTASEncuestaSatisfaccionEstudianteLinea = new List<TASEncuestaSatisfaccionEstudianteLineaEntity>();
            
            for (int cont=0; cont < etTASEncuestaSatisfaccionEstudianteDetalle.ltGradoSatisfaccion.Count; cont++)
            {
                ltTASEncuestaSatisfaccionEstudianteLinea.Add(new TASEncuestaSatisfaccionEstudianteLineaEntity {
                    SubCompania = (string)Session["SesionSubCompania"],
                    Programa = etTASEncuestaSatisfaccionEstudianteDetalle.etTASConfiguracionEncuestaSatisfaccionEstudiante.Programa,
                    Grupo = etTASEncuestaSatisfaccionEstudianteDetalle.etTASConfiguracionEncuestaSatisfaccionEstudiante.Grupo,
                    Fecha = etTASEncuestaSatisfaccionEstudianteDetalle.etTASConfiguracionEncuestaSatisfaccionEstudiante.Fecha,
                    EstudianteLic = (string)Session["Usuario"],
                    Linea = etTASEncuestaSatisfaccionEstudianteDetalle.ltLinea[cont],
                    CriterioEvaluacion = etTASEncuestaSatisfaccionEstudianteDetalle.ltLinea[cont],
                    Puntuacion = etTASEncuestaSatisfaccionEstudianteDetalle.ltGradoSatisfaccion[cont],
                    ObservacionSugerencia = etTASEncuestaSatisfaccionEstudianteDetalle.ObservacionSugerencias == null ? "" : etTASEncuestaSatisfaccionEstudianteDetalle.ObservacionSugerencias
                });
            }

            TASEncuestaSatisfaccionEstudianteSet setTASEncuestaSatisfaccionEstudiante = TASEncuestaSatisfaccionEstudiante.WebSeleccionar((string)Session["SesionSubCompania"], etTASEncuestaSatisfaccionEstudianteDetalle.etTASConfiguracionEncuestaSatisfaccionEstudiante.Programa, etTASEncuestaSatisfaccionEstudianteDetalle.etTASConfiguracionEncuestaSatisfaccionEstudiante.Grupo, etTASEncuestaSatisfaccionEstudianteDetalle.etTASConfiguracionEncuestaSatisfaccionEstudiante.Fecha, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            setTASEncuestaSatisfaccionEstudiante.ltTASEncuestaSatisfaccionEstudianteLinea.AddRange(ltTASEncuestaSatisfaccionEstudianteLinea);
            setTASEncuestaSatisfaccionEstudiante.ltTASEncuestaSatisfaccionEstudiante.First().UEstado = TASEncuestaSatisfaccionEstudianteService.Estado.Modified;

            long NumError = TASEncuestaSatisfaccionEstudiante.WebGuardar(setTASEncuestaSatisfaccionEstudiante, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return RedirectToAction("TASEstudianteEncuestaSatisfaccionEstudiante");
        }

        public ActionResult TASEstudianteEncuestaAlumnoInstructor()
        {
            TASConfiguracionEncuestaAlumnoInstructorInterfaceClient TASConfiguracionEncuestaAlumnoInstructor = new TASConfiguracionEncuestaAlumnoInstructorInterfaceClient();
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            TASEncuestaAlumnoInstructorInterfaceClient TASEncuestaAlumnoInstructor = new TASEncuestaAlumnoInstructorInterfaceClient();

            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionar((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASEstudianteEntity etTASEstudiante = ltTASEstudiante.FirstOrDefault();

            List<TASConfiguracionEncuestaAlumnoInstructorEntity> ltTASConfiguracionEncuestaAlumnoInstructor = TASConfiguracionEncuestaAlumnoInstructor.WebTASConfiguracionEncuestaAlumnoInstructorSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASConfiguracionEncuestaAlumnoInstructor = ltTASConfiguracionEncuestaAlumnoInstructor.OrderBy(o => o.Fecha).Where(x => etTASEstudiante.Programa == x.Programa && x.Grupo == etTASEstudiante.Grupo).ToList();

            List<TASConfiguracionEncuestaAlumnoInstructorEntity> ltEncuestasPendientes = new List<TASConfiguracionEncuestaAlumnoInstructorEntity>();
            //si existe configuraciones, entonces iterar y chequear si tiene alguna pendiente de hacer entre todas las que hay
            foreach (TASConfiguracionEncuestaAlumnoInstructorEntity etTASConfiguracionEncuestaAlumnoInstructor in ltTASConfiguracionEncuestaAlumnoInstructor)
            {
                //checkeamos si el estudiante realizo la encuesta
                TASEncuestaAlumnoInstructorSet setTASEncuestaAlumnoInstructor = TASEncuestaAlumnoInstructor.WebSeleccionar(etTASConfiguracionEncuestaAlumnoInstructor.SubCompania, etTASConfiguracionEncuestaAlumnoInstructor.Programa, etTASConfiguracionEncuestaAlumnoInstructor.Grupo, etTASConfiguracionEncuestaAlumnoInstructor.Materia, etTASConfiguracionEncuestaAlumnoInstructor.InstructorLic, etTASConfiguracionEncuestaAlumnoInstructor.Fecha, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (setTASEncuestaAlumnoInstructor.ltTASEncuestaAlumnoInstructorLinea.Count > 0)
                {
                    long RespuestasEncuestaEstudiante = setTASEncuestaAlumnoInstructor.ltTASEncuestaAlumnoInstructorLinea.Where(x => x.EstudianteLic == (string)Session["Usuario"]).Count();
                    if (RespuestasEncuestaEstudiante == 0)
                    {
                        ltEncuestasPendientes.Add(etTASConfiguracionEncuestaAlumnoInstructor);
                    }
                }
                else
                {
                    ltEncuestasPendientes.Add(etTASConfiguracionEncuestaAlumnoInstructor);
                }

            }

            //si no existe mandar a una pagina de mensaje
            if (ltEncuestasPendientes.Count == 0)
            {
                return View("TASEstudianteEncuestaSatisfaccionEstudianteRealizado");
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
                        new ListItem { Text = "Siempre" , Value = "4" },
                        new ListItem { Text = "Casi Siempre" , Value = "3" },
                        new ListItem { Text = "Algunas Veces" , Value = "2" },
                        new ListItem { Text = "Nunca" , Value = "1" },
                   }, "Value", "Text");

                ViewBag.ltGradoSatisfaccion = listItems;

                //for (int i= 0; i < arrayGradoSatisfaccion.Length; i++ )
                //{
                //    //ltGradoSatisfaccion.Add(new GradoSatisfaccionClass {
                //        Puntuacion = i,
                //        Descripcion = arrayGradoSatisfaccion[i]
                //    //});
                //}

                TASEncuestaAlumnoInstructorDetalle etTASEncuestaAlumnoInstructorDetalle = new TASEncuestaAlumnoInstructorDetalle
                {
                    etTASConfiguracionEncuestaAlumnoInstructor = ltEncuestasPendientes.FirstOrDefault(),
                    ltCriterioEvaluacion = ltTASCriterioEvaluacionAlumnoInstructor.OrderBy(o => o.Linea).Select(x => x.Descripcion).ToList(),
                    ltLinea = ltTASCriterioEvaluacionAlumnoInstructor.OrderBy(o => o.Linea).Select(x => x.Linea).ToList(),
                    ltGradoSatisfaccion = ltTASCriterioEvaluacionAlumnoInstructor.OrderBy(o => o.Linea).Select(x => x.Entero1).ToList(),
                    ObservacionSugerencias = ""
                };

                return View("TASEstudianteEncuestaAlumnoInstructorEncuestar", etTASEncuestaAlumnoInstructorDetalle);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TASEstudianteEncuestaAlumnoInstructor(TASEncuestaAlumnoInstructorDetalle etTASEncuestaAlumnoInstructorDetalle)
        {
            TASEncuestaAlumnoInstructorInterfaceClient TASEncuestaAlumnoInstructor = new TASEncuestaAlumnoInstructorInterfaceClient();
            List<TASEncuestaAlumnoInstructorLineaEntity> ltTASEncuestaAlumnoInstructorLinea = new List<TASEncuestaAlumnoInstructorLineaEntity>();

            for (int cont = 0; cont < etTASEncuestaAlumnoInstructorDetalle.ltGradoSatisfaccion.Count; cont++)
            {
                ltTASEncuestaAlumnoInstructorLinea.Add(new TASEncuestaAlumnoInstructorLineaEntity
                {
                    SubCompania = (string)Session["SesionSubCompania"],
                    Programa = etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Programa,
                    Grupo = etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Grupo,
                    Materia = etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Materia,
                    InstructorNroLicencia = etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.InstructorLic,
                    Fecha = etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Fecha,
                    EstudianteLic = (string)Session["Usuario"],
                    Linea = etTASEncuestaAlumnoInstructorDetalle.ltLinea[cont],
                    CriterioEvaluacion = etTASEncuestaAlumnoInstructorDetalle.ltLinea[cont],
                    Puntuacion = etTASEncuestaAlumnoInstructorDetalle.ltGradoSatisfaccion[cont],
                    ObservacionSugerencia = etTASEncuestaAlumnoInstructorDetalle.ObservacionSugerencias == null ? "" : etTASEncuestaAlumnoInstructorDetalle.ObservacionSugerencias
                });
            }

            TASEncuestaAlumnoInstructorSet setTASEncuestaAlumnoInstructor = TASEncuestaAlumnoInstructor.WebSeleccionar((string)Session["SesionSubCompania"], etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Programa, etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Grupo, etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Materia, etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.InstructorLic, etTASEncuestaAlumnoInstructorDetalle.etTASConfiguracionEncuestaAlumnoInstructor.Fecha,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            setTASEncuestaAlumnoInstructor.ltTASEncuestaAlumnoInstructorLinea.AddRange(ltTASEncuestaAlumnoInstructorLinea);
            setTASEncuestaAlumnoInstructor.ltTASEncuestaAlumnoInstructor.First().UEstado = TASEncuestaAlumnoInstructorService.Estado.Modified;

            long NumError = TASEncuestaAlumnoInstructor.WebGuardar(setTASEncuestaAlumnoInstructor, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return RedirectToAction("TASEstudianteEncuestaAlumnoInstructor");
        }
    }
}