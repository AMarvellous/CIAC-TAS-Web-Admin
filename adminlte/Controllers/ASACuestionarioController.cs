using adminlte.ASAConfiguracionService;
using adminlte.ASAGrupoPreguntaService;
using adminlte.ASAPreguntaService;
using adminlte.ASATransaccionService;
using adminlte.ASATReportePreguntaService;
using adminlte.ASATRespuestaService;
using adminlte.AXFSesionService;
using adminlte.AXFUsuarioService;
using adminlte.Models;
using adminlte.TASEstudianteService;
using adminlte.TASGrupoService;
using CrystalDecisions.CrystalReports.Engine;
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace adminlte.Controllers
{
    public class ASACuestionarioController : BaseController
    {
        // GET: ASACuestionario
        public ActionResult ASACuestionarioInicio()
        {
            bool ExisteASA = false;
            DateTime dtFechaIni = DateTime.Today;
            DateTime dtFechaFin = DateTime.Today;
            //bool ExisteHolgura = false;
            //DateTime dtFechaIniHolgura = DateTime.Today;
            //DateTime dtFechaFinHolgura = DateTime.Today;

            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            TASEstudianteSet setTASEstudiante = TASEstudiante.WebSeleccionar((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (setTASEstudiante.ltTASEstudiante.Count > 0)
            {
                //si existe el estudiante
                TASEstudianteEntity etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();
                //string Grupo = etTASEstudiante.Grupo;
                //verificamos si existe algun grupo activo en la configuracion del asa
                ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(etTASEstudiante.SubCompania, etTASEstudiante.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                DateTime dtHoraServidor = AXFSesion.WebObtenerHoraServidor();
                
                foreach (var item in setASAConfiguracion.ltASAConfiguracionLinea)
                {
                    //if (item.FechaDoc.Date == dtHoraServidor.Date)
                    //{
                    //si son mismas fechas comparar el rango de las horas
                    // Assuming you know d2 > d1
                    if (dtHoraServidor.Ticks > item.FechaIni.Ticks && dtHoraServidor.Ticks < item.FechaFin.Ticks)
                    {
                        // targetDt is in between d1 and d2
                        ExisteASA = true;
                        dtFechaIni = item.FechaIni;
                        dtFechaFin = item.FechaFin;
                        break;
                    }
                    //else if (dtHoraServidor.Ticks > item.FechaIni.Ticks && dtHoraServidor.Ticks < item.FechaFin.AddHours(1).Ticks)
                    //{
                    //    ExisteHolgura = true;
                    //    dtFechaIniHolgura = item.FechaIni;
                    //    dtFechaFinHolgura = item.FechaFin.AddHours(1);
                        
                    //}
                    //}
                }
            }

            List<ASAGrupoPreguntaEntity> ltASAGrupoPregunta = ASAGrupoPregunta.WebASAGrupoPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltASAGrupoPregunta = ltASAGrupoPregunta.Select(s => s.GrupoPregunta).ToList();

            SelectList ltSelectExamen = new SelectList(
            new List<ListItem>
            {
                new ListItem { Text = "Examen ASA" , Value = "ASACuestionarioExamen" }
            }, "Value", "Text");

            SelectList ltSelectPractica = new SelectList(
            new List<ListItem>
            {
                new ListItem { Text = "Practica ASA" , Value = "ASACuestionarioEntrada" }
            }, "Value", "Text");

            if (ExisteASA)
            {
                ViewBag.ltSelect = ltSelectExamen;
                ViewBag.ExisteASA = true;
                //Y si existe cuestionario anterior verificar si es examen, si es examen redireccionamos al examen, si no redireccionamos al menu de examen
                ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
                ASATransaccionInterfaceClient ASATransaccion = new ASATransaccionInterfaceClient();
                //double CriterioXPregunta = 0.6;
                bool ExisteCuestionarioPrevio = ASATRespuesta.WebASATRespuestaExiste((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (ExisteCuestionarioPrevio)
                {
                    bool EsExamen = ASATRespuesta.WebASATRespuestaEsExamen((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (EsExamen)
                    {
                        return RedirectToAction("ASACuestionarioExamen");
                    }
                    else
                    {
                        //si no es examen verifico si es la primera vez de examen, 1era vez examen: cierro, no es 1era vez : continuo a las practicas
                        bool PrimeraVez = ASATransaccion.WebASATransaccionEsPrimeraVez((string)Session["Usuario"], dtFechaIni, dtFechaFin, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        if (PrimeraVez)
                        {
                            //si es 1era vez examen: cierro
                            bool RespCerrado = ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        }
                        else
                        {
                            //no es la 1era vez : continuo a las practicas
                            Session["GrupoPregunta"] = new List<string>();
                            Session["PreguntaIni"] = 0;
                            Session["PreguntaFin"] = 0;
                            return RedirectToAction("ASACuestionarioEntrada");
                        }

                    }
                }
                else
                {
                    //Si no hay examen previo verificamos que sea la primerz vez que realizara el examen
                    bool PrimeraVez = ASATransaccion.WebASATransaccionEsPrimeraVez((string)Session["Usuario"], dtFechaIni, dtFechaFin, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (!PrimeraVez)
                    {
                        ViewBag.ltSelect = ltSelectPractica;
                        ViewBag.ExisteASA = false;
                    }
                }
            }
            else
            {
                ViewBag.ltSelect = ltSelectPractica;
                ViewBag.ExisteASA = false;
                //Si no existe ASA verificamos si existe formulario anterior para mandarlo directo al formulario de preguntas de examen o practica dependiendo de la hora y si existen respuestas
                ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
                double CriterioXPregunta = 0.6;
                bool ExisteCuestionarioPrevio = ASATRespuesta.WebASATRespuestaExiste((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (ExisteCuestionarioPrevio)
                {
                    bool EsExamen = ASATRespuesta.WebASATRespuestaEsExamen((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (EsExamen)
                    {
                        //Verificamos si todavia es un examen valido dentro del tiempo establecido, sino cerramos examen
                        long TiempoMaximo = 60;
                        bool ExamenValido = ASATRespuesta.WebASATRespuestaSiCumpleHorarioExamen((string)Session["Usuario"], TiempoMaximo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        if (ExamenValido)
                        {
                            //existe posiblemente olgura de este examen
                            return RedirectToAction("ASACuestionarioExamen");
                        }
                        else
                        {
                            ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        }
                    }
                    else
                    {
                        bool SiCumpleHorario = ASATRespuesta.WebASATRespuestaSiCumpleHorario((string)Session["Usuario"], CriterioXPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        if (SiCumpleHorario)
                        {
                            Session["GrupoPregunta"] = new List<string>();
                            Session["PreguntaIni"] = 0;
                            Session["PreguntaFin"] = 0;
                            return RedirectToAction("ASACuestionarioEntrada");
                        }
                    }

                }
            }


            return View();
        }

        [HttpPost]
        public ActionResult ASACuestionarioInicio(string Cuestionario, List<string> GrupoPregunta, int PreguntaIni = 0, int PreguntaFin = 0)
        {
            if (GrupoPregunta == null)
            {
                GrupoPregunta = new List<string>();
            }

            if (Cuestionario == "ASACuestionarioExamen")
            {
                return RedirectToAction(Cuestionario, "ASACuestionario");
            }
            else if (Cuestionario == "ASACuestionarioEntrada" && PreguntaIni <= PreguntaFin)
            {
                Session["GrupoPregunta"] = GrupoPregunta;
                Session["PreguntaIni"] = PreguntaIni;
                Session["PreguntaFin"] = PreguntaFin;
                return RedirectToAction(Cuestionario, "ASACuestionario");
            }
            else
            {
                return RedirectToAction("ASACuestionarioInicio", "ASACuestionario");
            }

        }

        [HttpGet]
        public ActionResult ASACuestionarioEntrada()
        {
            List<string> GrupoPregunta = (List<string>)Session["GrupoPregunta"];
            int PreguntaIni = (int)Session["PreguntaIni"];
            int PreguntaFin = (int)Session["PreguntaFin"];
            //UPDATE METODO WebASATRespuestaExiste
            //UPDATE METODO WebASATRespuestaSeleccionarSiCumpleHorario
            //UPDATE METODO WebASAPreguntaSeleccionarTodo
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            Random Random = new Random();
            bool FiltrarPreguntasIniFin = false;
            int NumeroPreguntas = 100;
            double CriterioXPregunta = 0.6;

            if (PreguntaIni > 0)
            {
                if (PreguntaIni < PreguntaFin)
                {
                    NumeroPreguntas = PreguntaFin - PreguntaIni + 1;
                    FiltrarPreguntasIniFin = true;

                    if (NumeroPreguntas > 100)
                    {
                        NumeroPreguntas = 100;
                    }

                }
            }
            List<ASAPreguntaSet> ltsetASAPregunta = new List<ASAPreguntaSet>();
            //Verificamos si ya abrio una pregunta ASA, 
            //Si se abrio anteriormente cuestionario verificar primero la hora, si aun esta dentro el horario recuperar y llenar formulario con anteriores preguntas ,fuera de horario cerrar y empezar otro
            //NO se abrio entonces inicialimos un nuevo cuestionario con las opciones en 0

            bool ExisteCuestionarioPrevio = ASATRespuesta.WebASATRespuestaExiste((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (ExisteCuestionarioPrevio)
            {
                bool SiCumpleHorario = ASATRespuesta.WebASATRespuestaSiCumpleHorario((string)Session["Usuario"], CriterioXPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (SiCumpleHorario)
                {
                    ltsetASAPregunta = ASAPregunta.WebASATRespuestaSeleccionarPreguntaXEstudiante((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    long TiempoMaximo = 60;
                    if (ltsetASAPregunta.Count == 100)
                    {
                        TiempoMaximo = 60;
                    }
                    else
                    {
                        var tiempo = Math.Ceiling(ltsetASAPregunta.Count * CriterioXPregunta);
                        TiempoMaximo = Convert.ToInt64(tiempo);
                    }

                    ViewBag.NumeroPreguntas = ltsetASAPregunta.Count;
                    ViewBag.TiempoRestante = ASATRespuesta.WebASATRespuestaTiempoRestante((string)Session["Usuario"], TiempoMaximo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
                else
                {
                    //cerramos el anterior
                    bool RespCerrado = ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (RespCerrado)
                    {
                        List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        if (FiltrarPreguntasIniFin)
                        {
                            if (GrupoPregunta.Count > 0)
                            {
                                ltASAPregunta = ltASAPregunta.Where(x => GrupoPregunta.Contains(x.GrupoPregunta) && x.NroPregunta >= PreguntaIni && x.NroPregunta <= PreguntaFin).ToList();
                            }
                            else
                            {
                                ltASAPregunta = ltASAPregunta.Where(x => x.NroPregunta >= PreguntaIni && x.NroPregunta <= PreguntaFin).ToList();
                            }
                        }
                        else
                        {
                            if (GrupoPregunta.Count > 0)
                            {
                                ltASAPregunta = ltASAPregunta.Where(x => GrupoPregunta.Contains(x.GrupoPregunta)).ToList();
                            }

                        }

                        ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(NumeroPreguntas).ToList();
                        var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta, false, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        if (RespGuardado == 0)
                        {
                            ltASAPregunta = (from Pregunta in ltASAPregunta
                                             orderby Pregunta.NroPregunta ascending
                                             select Pregunta).ToList();
                            foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                            {
                                ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                            }

                            //Calculo en criterio al numero de preguntas
                            ViewBag.NumeroPreguntas = ltASAPregunta.Count;
                            if (ltASAPregunta.Count == 100)
                            {
                                ViewBag.TiempoRestante = 60;
                            }
                            else
                            {
                                var tiempo = Math.Ceiling(ltASAPregunta.Count * CriterioXPregunta);
                                ViewBag.TiempoRestante = Convert.ToInt32(tiempo);
                            }

                        }
                    }
                }
            }
            else
            {
                List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (FiltrarPreguntasIniFin)
                {
                    if (GrupoPregunta.Count > 0)
                    {
                        ltASAPregunta = ltASAPregunta.Where(x => GrupoPregunta.Contains(x.GrupoPregunta) && x.NroPregunta >= PreguntaIni && x.NroPregunta <= PreguntaFin).ToList();
                    }
                    else
                    {
                        ltASAPregunta = ltASAPregunta.Where(x => x.NroPregunta >= PreguntaIni && x.NroPregunta <= PreguntaFin).ToList();
                    }
                }
                else
                {
                    if (GrupoPregunta.Count > 0)
                    {
                        ltASAPregunta = ltASAPregunta.Where(x => GrupoPregunta.Contains(x.GrupoPregunta)).ToList();
                    }

                }

                ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(NumeroPreguntas).ToList();
                var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta, false, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (RespGuardado == 0)
                {
                    ltASAPregunta = (from Pregunta in ltASAPregunta
                                     orderby Pregunta.NroPregunta ascending
                                     select Pregunta).ToList();
                    foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                    {
                        ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                    }

                    //Calculo en criterio al numero de preguntas
                    ViewBag.NumeroPreguntas = ltASAPregunta.Count;
                    if (ltASAPregunta.Count == 100)
                    {
                        ViewBag.TiempoRestante = 60;
                    }
                    else
                    {
                        var tiempo = Math.Ceiling(ltASAPregunta.Count * CriterioXPregunta);
                        ViewBag.TiempoRestante = Convert.ToInt32(tiempo);
                    }

                }

            }



            // Tab Data
            ThumbnailViewModel model = new ThumbnailViewModel();
            model.ThumbnailModelList = new List<ThumbnailModel>();

            // Test Details Data 
            List<ASAPreguntaSet> _detaisllist = new List<ASAPreguntaSet>();
            //ltsetASAPregunta = ltsetASAPregunta.OrderBy(O => O.ltASAPregunta.OrderBy( x => x.NroPregunta)).ToList();
            //SortedSet<ASAPreguntaSet> A = new SortedSet<ASAPreguntaSet>();

            foreach (ASAPreguntaSet setASAPregunta in ltsetASAPregunta)
            {
                //A.Add(setASAPregunta);
                _detaisllist.Add(setASAPregunta);
            }
            //for (int i = 0; i < count; i++)
            //{
            //    ASAPreguntaSet obj = new ASAPreguntaSet();
            //    //obj.ltASAPregunta = new List<ASAPreguntaEntity>() { new ASAPreguntaEntity { Pregunta = "117 Un motor eléctrico de 12 voltios tiene 1.000 watts de entrada y un rendimiento de un caballo de fuerza. Manteniendo la misma eficiencia, ¿cuánta de energía de entrada requerirá un motor eléctrico de 24-voltios y de un caballo de fuerza? (Nota: 1 caballo de fuerza = 746 watts) " + i, NroPregunta = i } };
            //    //obj.ltASAPreguntaRespuesta = new List<ASAPreguntaRespuestaEntity> {
            //    //    new ASAPreguntaRespuestaEntity { NroPregunta = i, Opcion = i, Respuesta = "igual al voltaje aplicado más alto. "+i, Correcto =true },
            //    //    new ASAPreguntaRespuestaEntity { NroPregunta = i, Opcion = i+1, Respuesta = "Por lo menos 20 por ciento más que el voltaje aplicado más alto."+i+1, Correcto =false },
            //    //    new ASAPreguntaRespuestaEntity { NroPregunta = i, Opcion = i+2, Respuesta = "Un circuito de luz anticolisión de 24 voltios, que consiste de dos conjuntos de luces, que requieren cada uno 3 amperios durante su funcionamiento."+i+1, Correcto =false }
            //    //};

            //    obj = ASAPregunta.WebSeleccionar(ltASAPregunta[i].NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //    _detaisllist.Add(obj);
            //}

            // batch your List data for tab view i want batch by 2 you can set your value

            var listOfBatches = _detaisllist.Batch(1);

            int tabNo = 1;

            foreach (var batchItem in listOfBatches)
            {
                // Generating tab
                ThumbnailModel obj = new ThumbnailModel();
                obj.ThumbnailLabel = "Lebel" + tabNo;
                obj.ThumbnailTabId = "tab" + tabNo;
                obj.ThumbnailTabNo = tabNo;
                obj.Thumbnail_Aria_Controls = "tab" + tabNo;
                obj.Thumbnail_Href = "#tab" + tabNo;

                // batch details

                obj.ltASAPregunta = new List<ASAPreguntaSet>();

                foreach (var item in batchItem)
                {
                    ASAPreguntaSet detailsObj = new ASAPreguntaSet();
                    detailsObj = item;
                    obj.ltASAPregunta.Add(detailsObj);
                }

                model.ThumbnailModelList.Add(obj);

                tabNo++;
            }

            // Getting first tab data
            var first = model.ThumbnailModelList.FirstOrDefault();

            // Getting first tab data
            var last = model.ThumbnailModelList.LastOrDefault();

            foreach (var item in model.ThumbnailModelList)
            {
                if (item.ThumbnailTabNo == first.ThumbnailTabNo)
                {
                    item.Thumbnail_ItemPosition = "first";
                }

                if (item.ThumbnailTabNo == last.ThumbnailTabNo)
                {
                    item.Thumbnail_ItemPosition = "last";
                }

            }


            ////Verificamos si es primera vez o ya se abrio anteriormente el cuestinario
            //bool PrimeraVez = false;
            //if (PrimeraVez)
            //{
            //    //Formulario en blanco y datetime en 0

            //}
            //else
            //{
            //    //Formulario ya seleccionado y con el datetime desde donde se quedo

            //}

            return View(model);
        }

        [HttpPost]
        public ActionResult ASACuestionarioEntrada(ThumbnailViewModel Model)
        {
            return View();
        }

        public JsonResult ActualizarRespuesta(string Respuesta)
        {
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            long NumError = 0;
            if (Respuesta != null)
            {
                string[] Split = Respuesta.Split('-');
                if (Split.Length == 2)
                {
                    //Guardamos la Respuesta
                    long NroPregunta = 0;
                    long Opt = 0;
                    string NumeroPregunta = Split[0];
                    string Opcion = Split[1];
                    string Estudiante = (string)Session["Usuario"];
                    long.TryParse(NumeroPregunta, out NroPregunta);
                    long.TryParse(Opcion, out Opt);
                    NumError = ASATRespuesta.WebGuardarRespuesta(Estudiante, NroPregunta, Opt, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }

            }

            return Json(new { success = true });
        }

        public JsonResult CerrarCuestionario(string[] Respuesta)
        {
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            long NumError = 0;
            if (Respuesta != null)
            {
                foreach (string resp in Respuesta)
                {
                    string[] Split = resp.Split('-');
                    if (Split.Length == 2)
                    {
                        //Guardamos la Respuesta
                        long NroPregunta = 0;
                        long Opt = 0;
                        string NumeroPregunta = Split[0];
                        string Opcion = Split[1];
                        string Estudiante = (string)Session["Usuario"];
                        long.TryParse(NumeroPregunta, out NroPregunta);
                        long.TryParse(Opcion, out Opt);
                        NumError = ASATRespuesta.WebGuardarRespuesta(Estudiante, NroPregunta, Opt, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    }
                }

                ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            }


            return Json(new { success = true, url = Url.Action("Index", "Index") });
        }
        [HttpGet]
        public ActionResult ASACuestionarioExamen()
        {
            //long TiempoRestante = 0;
            DateTime FechaFin = DateTime.Now;
            DateTime FechaIni = DateTime.Now;
            DateTime dtHoraServidor = DateTime.Now;
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            ASATransaccionInterfaceClient ASATransaccion = new ASATransaccionInterfaceClient();
            Random Random = new Random();
            bool FiltrarPreguntasIniFin = false;
            int NumeroPreguntas = 100;
            double CriterioXPregunta = 0.6;
            bool ExisteASAConfig = false;
            DateTime dtFechaIniHolgura = DateTime.Today;
            DateTime dtFechaFinHolgura = DateTime.Today;

            TASEstudianteSet setTASEstudiante = TASEstudiante.WebSeleccionar((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (setTASEstudiante.ltTASEstudiante.Count > 0)
            {
                //si existe el estudiante
                TASEstudianteEntity etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();
                string Grupo = etTASEstudiante.Grupo;
                //verificamos si existe algun grupo activo en la configuracion del asa
                ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(etTASEstudiante.SubCompania, etTASEstudiante.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                dtHoraServidor = AXFSesion.WebObtenerHoraServidor();
                foreach (var item in setASAConfiguracion.ltASAConfiguracionLinea)
                {
                    //if (item.FechaDoc.Date == dtHoraServidor.Date)
                    //{
                    //si son mismas fechas comparar el rango de las horas
                    // Assuming you know d2 > d1
                    if (dtHoraServidor.Ticks > item.FechaIni.Ticks && dtHoraServidor.Ticks < item.FechaFin.Ticks)
                    {
                        //double Minutos = (item.FechaFin - item.FechaIni).TotalMinutes;
                        //TiempoMaximo = Convert.ToInt64(Minutos);

                        //double MinutosRestante = (dtHoraServidor - item.FechaIni).TotalMinutes;
                        //TiempoRestante = TiempoMaximo - Convert.ToInt64(MinutosRestante);                        
                        FechaFin = item.FechaFin;
                        FechaIni = item.FechaIni;
                        NumeroPreguntas = Convert.ToInt32(item.CantidadPregunta);
                        ExisteASAConfig = true;
                        break;
                    }
                    //else if (dtHoraServidor.Ticks > item.FechaIni.Ticks && dtHoraServidor.Ticks < item.FechaFin.AddHours(1).Ticks)
                    //{
                    //    ExisteHolgura = true;
                    //    dtFechaIniHolgura = item.FechaIni;
                    //    dtFechaFinHolgura = item.FechaFin.AddHours(1);
                    //    NumeroPreguntas = Convert.ToInt32(item.CantidadPregunta);
                    //}
                    //}
                }
            }

            List<ASAPreguntaSet> ltsetASAPregunta = new List<ASAPreguntaSet>();
            //Verificamos si ya abrio una pregunta ASA, 
            //Si se abrio anteriormente cuestionario verificar primero la hora, si aun esta dentro el horario recuperar y llenar formulario con anteriores preguntas ,fuera de horario cerrar y empezar otro
            //NO se abrio entonces inicialimos un nuevo cuestionario con las opciones en 0

            bool ExisteCuestionarioPrevio = ASATRespuesta.WebASATRespuestaExiste((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (ExisteCuestionarioPrevio)
            {                
                long TiempoMaximo = 60;
                bool SiCumpleHorario = ASATRespuesta.WebASATRespuestaSiCumpleHorarioExamen((string)Session["Usuario"], TiempoMaximo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (SiCumpleHorario)
                {
                    ltsetASAPregunta = ASAPregunta.WebASATRespuestaSeleccionarPreguntaXEstudiante((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                    ViewBag.NumeroPreguntas = ltsetASAPregunta.Count;
                    ViewBag.TiempoRestante = ASATRespuesta.WebASATRespuestaTiempoRestanteExamen((string)Session["Usuario"], TiempoMaximo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
                else
                {
                    //cerramos el anterior
                    bool RespCerrado = ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (RespCerrado)
                    {
                        return RedirectToAction("ASACuestionarioInicio");
                    }
                }
            }
            else
            {
                //Verificamos que este dentro del horario de ASA
                if (ExisteASAConfig)
                {
                    //Verificamos si es la primera vez
                    bool PrimeraVez = ASATransaccion.WebASATransaccionEsPrimeraVez((string)Session["Usuario"], FechaIni, FechaFin, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (PrimeraVez)
                    {
                        List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                        ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(NumeroPreguntas).ToList();
                        var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta, true, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        if (RespGuardado == 0)
                        {
                            ltASAPregunta = (from Pregunta in ltASAPregunta
                                             orderby Pregunta.NroPregunta ascending
                                             select Pregunta).ToList();
                            foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                            {
                                ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                            }

                            //Calculo en criterio al numero de preguntas
                            ViewBag.NumeroPreguntas = ltASAPregunta.Count;
                            ViewBag.TiempoRestante = 60;
                            //double TotalMin = (FechaFin.AddHours(1) - dtHoraServidor).TotalMinutes;
                            //long Minutos = Convert.ToInt64(TotalMin);
                            //if (Minutos > 60)
                            //{
                            //    ViewBag.TiempoRestante = 60;
                            //}
                            //else
                            //{
                            //    ViewBag.TiempoRestante = Minutos;
                            //}
                        }
                    }
                    else
                    {
                        return RedirectToAction("ASACuestionarioInicio");
                    }
                }
                else
                {
                    return RedirectToAction("ASACuestionarioInicio");
                }

            }



            // Tab Data
            ThumbnailViewModel model = new ThumbnailViewModel();
            model.ThumbnailModelList = new List<ThumbnailModel>();

            // Test Details Data 
            List<ASAPreguntaSet> _detaisllist = new List<ASAPreguntaSet>();
            //ltsetASAPregunta = ltsetASAPregunta.OrderBy(O => O.ltASAPregunta.OrderBy( x => x.NroPregunta)).ToList();
            //SortedSet<ASAPreguntaSet> A = new SortedSet<ASAPreguntaSet>();

            foreach (ASAPreguntaSet setASAPregunta in ltsetASAPregunta)
            {
                //A.Add(setASAPregunta);
                _detaisllist.Add(setASAPregunta);
            }
            //for (int i = 0; i < count; i++)
            //{
            //    ASAPreguntaSet obj = new ASAPreguntaSet();
            //    //obj.ltASAPregunta = new List<ASAPreguntaEntity>() { new ASAPreguntaEntity { Pregunta = "117 Un motor eléctrico de 12 voltios tiene 1.000 watts de entrada y un rendimiento de un caballo de fuerza. Manteniendo la misma eficiencia, ¿cuánta de energía de entrada requerirá un motor eléctrico de 24-voltios y de un caballo de fuerza? (Nota: 1 caballo de fuerza = 746 watts) " + i, NroPregunta = i } };
            //    //obj.ltASAPreguntaRespuesta = new List<ASAPreguntaRespuestaEntity> {
            //    //    new ASAPreguntaRespuestaEntity { NroPregunta = i, Opcion = i, Respuesta = "igual al voltaje aplicado más alto. "+i, Correcto =true },
            //    //    new ASAPreguntaRespuestaEntity { NroPregunta = i, Opcion = i+1, Respuesta = "Por lo menos 20 por ciento más que el voltaje aplicado más alto."+i+1, Correcto =false },
            //    //    new ASAPreguntaRespuestaEntity { NroPregunta = i, Opcion = i+2, Respuesta = "Un circuito de luz anticolisión de 24 voltios, que consiste de dos conjuntos de luces, que requieren cada uno 3 amperios durante su funcionamiento."+i+1, Correcto =false }
            //    //};

            //    obj = ASAPregunta.WebSeleccionar(ltASAPregunta[i].NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //    _detaisllist.Add(obj);
            //}

            // batch your List data for tab view i want batch by 2 you can set your value

            var listOfBatches = _detaisllist.Batch(1);

            int tabNo = 1;

            foreach (var batchItem in listOfBatches)
            {
                // Generating tab
                ThumbnailModel obj = new ThumbnailModel();
                obj.ThumbnailLabel = "Lebel" + tabNo;
                obj.ThumbnailTabId = "tab" + tabNo;
                obj.ThumbnailTabNo = tabNo;
                obj.Thumbnail_Aria_Controls = "tab" + tabNo;
                obj.Thumbnail_Href = "#tab" + tabNo;

                // batch details

                obj.ltASAPregunta = new List<ASAPreguntaSet>();

                foreach (var item in batchItem)
                {
                    ASAPreguntaSet detailsObj = new ASAPreguntaSet();
                    detailsObj = item;
                    obj.ltASAPregunta.Add(detailsObj);
                }

                model.ThumbnailModelList.Add(obj);

                tabNo++;
            }

            // Getting first tab data
            var first = model.ThumbnailModelList.FirstOrDefault();

            // Getting first tab data
            var last = model.ThumbnailModelList.LastOrDefault();

            foreach (var item in model.ThumbnailModelList)
            {
                if (item.ThumbnailTabNo == first.ThumbnailTabNo)
                {
                    item.Thumbnail_ItemPosition = "first";
                }

                if (item.ThumbnailTabNo == last.ThumbnailTabNo)
                {
                    item.Thumbnail_ItemPosition = "last";
                }

            }


            ////Verificamos si es primera vez o ya se abrio anteriormente el cuestinario
            //bool PrimeraVez = false;
            //if (PrimeraVez)
            //{
            //    //Formulario en blanco y datetime en 0

            //}
            //else
            //{
            //    //Formulario ya seleccionado y con el datetime desde donde se quedo

            //}

            return View(model);
        }
        public JsonResult CerrarCuestionarioExamen(string[] Respuesta)
        {
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            long NumError = 0;
            if (Respuesta != null)
            {
                foreach (string resp in Respuesta)
                {
                    string[] Split = resp.Split('-');
                    if (Split.Length == 2)
                    {
                        //Guardamos la Respuesta
                        long NroPregunta = 0;
                        long Opt = 0;
                        string NumeroPregunta = Split[0];
                        string Opcion = Split[1];
                        string Estudiante = (string)Session["Usuario"];
                        long.TryParse(NumeroPregunta, out NroPregunta);
                        long.TryParse(Opcion, out Opt);
                        NumError = ASATRespuesta.WebGuardarRespuesta(Estudiante, NroPregunta, Opt, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    }
                }

                ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            }


            return Json(new { success = true, url = Url.Action("Index", "Index") });
        }

        [HttpGet]
        public ActionResult ASACuestionarioPregunta()
        {
            SelectList ltSelect = new SelectList(
            new List<ListItem>
            {
                new ListItem { Text = "Agregar Mediante Excel" , Value = "ASACuestionarioExcel" },
                new ListItem { Text = "Agregar Manualmente" , Value = "ASACuestionarioListaPregunta"}
            }, "Value", "Text");

            ViewBag.ltSelect = ltSelect;

            return View();
        }

        [HttpPost]
        public ActionResult ASACuestionarioPregunta(string Cuestionario)
        {
            if (Cuestionario != "")
            {
                return RedirectToAction(Cuestionario, "ASACuestionario");
            }
            else
            {
                return RedirectToAction("ASACuestionarioPregunta", "ASACuestionario");
            }

        }


        [HttpGet]
        public ActionResult ASACuestionarioExcel()
        {
            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            List<ASAGrupoPreguntaEntity> ltASAGrupoPregunta = ASAGrupoPregunta.WebASAGrupoPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltSelect = ltASAGrupoPregunta.Select(s => s.GrupoPregunta).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult ASACuestionarioExcel(HttpPostedFileBase FileUpload, string GrupoPregunta)
        {
            long NumError = 0;
            if (FileUpload != null && FileUpload.ContentLength > 0 && GrupoPregunta != "")
            {
                // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                // to get started. This is how we avoid dependencies on ACE or Interop:
                Stream stream = FileUpload.InputStream;

                // We return the interface, so that
                IExcelDataReader reader = null;


                if (FileUpload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileUpload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    NumError = 1;
                }

                if (NumError == 0)
                {
                    reader.IsFirstRowAsColumnNames = true;
                    DataSet dsResult = reader.AsDataSet();
                    reader.Close();
                    if (dsResult.Tables.Count > 0)
                    {
                        NumError = VerificarFormatoExcel(dsResult.Tables[0]);
                        if (NumError == 0)
                        {
                            NumError = AgregrarExcelSet(dsResult.Tables[0], GrupoPregunta);
                        }
                    }
                }

            }

            if (NumError == 0)
            {
                return RedirectToAction("ASACuestionarioPregunta", "ASACuestionario");
            }
            else
            {
                return View();
            }

        }

        private long AgregrarExcelSet(DataTable dtExcel, string GrupoPregunta)
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaSet setASAPregunta;
            string Subcompania = (string)Session["SesionSubCompania"];
            long NumError = 0;
            long NroPregunta = 0;
            bool CorrectoR1 = false;
            bool CorrectoR2 = false;
            bool CorrectoR3 = false;

            try
            {

                if (dtExcel.Rows.Count > 0)
                {
                    //Limpiar las preguntas ASA Antiguas
                    NumError = ASAPregunta.WebEliminarTodosXGrupoPregunta(GrupoPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (NumError == 0)
                    {
                        foreach (DataRow dtrow in dtExcel.Rows)
                        {
                            setASAPregunta = ASAPregunta.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                            long.TryParse(dtrow.ItemArray[0].ToString(), out NroPregunta);
                            foreach (ASAPreguntaEntity etASAPregunta in setASAPregunta.ltASAPregunta)
                            {
                                etASAPregunta.NroPregunta = NroPregunta;
                                etASAPregunta.Pregunta = dtrow.ItemArray[1].ToString();
                                etASAPregunta.GrupoPregunta = GrupoPregunta;
                                etASAPregunta.UEstado = ASAPreguntaService.Estado.Modified;
                            }

                            List<ASAPreguntaRespuestaEntity> etASAPreguntaRespuesta = setASAPregunta.ltASAPreguntaRespuesta;
                            long Opcion = 1;
                            CorrectoR1 = dtrow.ItemArray[5].ToString() == "0" ? false : true;

                            etASAPreguntaRespuesta.Add(new ASAPreguntaRespuestaEntity
                            {
                                NroPregunta = NroPregunta,
                                Opcion = Opcion,
                                Respuesta = dtrow.ItemArray[2].ToString(),
                                Correcto = CorrectoR1,
                                TipoPregunta = "",
                                UEstado = ASAPreguntaService.Estado.Added
                            });

                            Opcion++;
                            CorrectoR2 = dtrow.ItemArray[6].ToString() == "0" ? false : true;
                            etASAPreguntaRespuesta.Add(new ASAPreguntaRespuestaEntity
                            {
                                NroPregunta = NroPregunta,
                                Opcion = Opcion,
                                Respuesta = dtrow.ItemArray[3].ToString(),
                                Correcto = CorrectoR2,
                                TipoPregunta = "",
                                UEstado = ASAPreguntaService.Estado.Added
                            });

                            Opcion++;
                            CorrectoR3 = dtrow.ItemArray[7].ToString() == "0" ? false : true;
                            etASAPreguntaRespuesta.Add(new ASAPreguntaRespuestaEntity
                            {
                                NroPregunta = NroPregunta,
                                Opcion = Opcion,
                                Respuesta = dtrow.ItemArray[4].ToString(),
                                Correcto = CorrectoR3,
                                TipoPregunta = "",
                                UEstado = ASAPreguntaService.Estado.Added
                            });

                            ASAPregunta.WebGuardar(setASAPregunta, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                        }

                    }

                }



            }
            catch (Exception)
            {
                NumError = 1;
            }

            return NumError;
        }

        private long VerificarFormatoExcel(DataTable dtExcel)
        {
            long NumError = 0;
            string[] FormatoExcel = { "Nro", "Pregunta", "Opcion1",
                "Opcion2","Opcion3","Respuesta1","Respuesta2","Respuesta3" };
            DataColumnCollection Columnas = dtExcel.Columns;

            foreach (var item in FormatoExcel)
            {
                if (!Columnas.Contains(item))
                {
                    NumError = 1;
                }
            }

            return NumError;
        }

        [HttpGet]
        public ActionResult ASACuestionarioGrafica(string Estudiante = "")
        {
            ASATransaccionInterfaceClient ASATransaccion = new ASATransaccionInterfaceClient();
            if (Estudiante == "")
            {
                Estudiante = (string)Session["Usuario"];
            }

            List<ASATransaccionEntity> ltASATransaccion = ASATransaccion.WebASATransaccionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            //var ltAgrupada = ltASATransaccion.Where(x => x.EstudianteCI == Estudiante).GroupBy(g => g.Intento).ToList();
            //var ltASATransaccionQuery = ltASATransaccion.Where(x => x.EstudianteCI == Estudiante).GroupBy(g => new { g.Intento, g.GrupoPregunta }).Select(s => s.ToList()).ToList();
            ////var ltASATransaccionQuery2 = ltASATransaccion.Where(x => x.EstudianteCI == Estudiante).GroupBy(g => new { Intento = g.Intento, GrupoPregunta = g.GrupoPregunta }).Select(s => new { Intento = s.Key.Intento, GrupoPregunta = s.Key.GrupoPregunta, Correcto = s.Count() }).ToList();
            //var ltASATransaccionQuery3 = ltASATransaccion
            //    .Where(x => x.EstudianteCI == Estudiante)
            //    .GroupBy(g => g.Intento)
            //    .Select(s =>  new 
            //    {
            //        Intento = s.Key,
            //        GrupoPregunta = s.GroupBy(x => x.GrupoPregunta).Select(s2 => s.ToList())
            //    }).ToList();

            //var q = from c in ltASATransaccion
            //        group c by c.Intento into g
            //        select new { g.Key, Count = g.Count(), SubGroups = from c in g group c by c.GrupoPregunta into g2 select g2 };

            //var ltAgrupada = ltASATransaccion.Where(x => x.EstudianteCI == Estudiante)
            //    .GroupBy(g => g.Intento)
            //    .Select(s => new
            //    {
            //        Intento = s.Key,
            //        GrupoPregunta = s.GroupBy(g => g.GrupoPregunta).Select(sb => new 
            //        {
            //            GrupoPregunta = sb.Key,
            //            Total = sb.Count()
            //        })
            //    }).ToList();

            //QUERY PARA EL DETALLE
            var ltAgrupada = ltASATransaccion.Where(x => x.EstudianteCI == Estudiante)
                .GroupBy(g => g.Intento)
                .Select(s => s.GroupBy(g => g.GrupoPregunta).Select(s2 => s2.ToList()).ToList()).ToList();

            ViewBag.ltASATransaccion = ltAgrupada;

            return View();
        }

        [HttpGet]
        public ActionResult ASACuestionarioGraficaEstudiante()
        {
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            List<AXFUsuarioClaseUsuarioEntity> ltAXFUsuarioClaseUsuario = AXFUsuario.WebAXFUsuarioClaseUsuarioSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltAXFUsuarioClaseUsuario = ltAXFUsuarioClaseUsuario.Where(x => x.ClaseUsuario == "Estudiante").ToList();
            ViewBag.ltAXFUsuarioClaseUsuario = ltAXFUsuarioClaseUsuario.Select(x => x.Usuario).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult ASACuestionarioGraficaEstudiante(string Estudiante)
        {
            if (Estudiante != "")
            {
                return RedirectToAction("ASACuestionarioGrafica", "ASACuestionario", new { Estudiante = Estudiante });
            }
            else
            {
                return RedirectToAction("ASACuestionarioGraficaEstudiante", "ASACuestionario");
            }
        }

        [HttpGet]
        public ActionResult ASACuestionarioListaPregunta()
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltASAPregunta);
        }

        [HttpGet]
        public ActionResult ASAPreguntaNuevo()
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaSet setASAPregunta = ASAPregunta.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            List<ASAGrupoPreguntaEntity> ltASAGrupoPregunta = ASAGrupoPregunta.WebASAGrupoPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltASAGrupoPregunta = ltASAGrupoPregunta.Select(s => s.GrupoPregunta).ToList();

            return View(setASAPregunta);
        }

        [HttpPost]
        public ActionResult ASAPreguntaNuevo(ASAPreguntaSet setASAPregunta)
        {
            long NumError = 0;
            long NroPregunta = 0;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();

            if (setASAPregunta.ltASAPregunta != null)
            {
                ASAPreguntaEntity etASAPregunta = setASAPregunta.ltASAPregunta.First();
                if (etASAPregunta.NroPregunta != 0 && etASAPregunta.GrupoPregunta != "" && etASAPregunta.Pregunta != "")
                {
                    ASAPreguntaSet setASAPreguntaNuevo = ASAPregunta.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    ASAPreguntaEntity etASAPreguntaNuevo = setASAPreguntaNuevo.ltASAPregunta.First();

                    etASAPreguntaNuevo.NroPregunta = etASAPregunta.NroPregunta;
                    etASAPreguntaNuevo.Pregunta = etASAPregunta.Pregunta;
                    etASAPreguntaNuevo.GrupoPregunta = etASAPregunta.GrupoPregunta;
                    etASAPreguntaNuevo.UEstado = ASAPreguntaService.Estado.Added;
                    NroPregunta = etASAPregunta.NroPregunta;
                    NumError = ASAPregunta.WebGuardar(setASAPreguntaNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                }
                else
                {
                    NumError = 1;
                }
            }
            else
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                return RedirectToAction("ASAPreguntaEditar", new { NroPregunta = NroPregunta });
            }
            else
            {
                return RedirectToAction("ASACuestionarioListaPregunta");
            }

        }

        public ActionResult ASAPreguntaEditar(long NroPregunta)
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaSet setASAPregunta = new ASAPreguntaSet();

            setASAPregunta = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            List<ASAGrupoPreguntaEntity> ltASAGrupoPregunta = ASAGrupoPregunta.WebASAGrupoPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltASAGrupoPregunta = ltASAGrupoPregunta.Select(s => s.GrupoPregunta).ToList();

            //Verificamos si existe una imagen para esta pregunta
            var etASAPregunta = setASAPregunta.ltASAPregunta.First();
            if (etASAPregunta.Texto0 != "")
            {
                //cargamos imagen
                ViewBag.ExisteImagen = true;
                //var path = etASAPregunta.Texto0.Substring(0, etASAPregunta.Texto0.IndexOf("@"));
                ViewBag.PathImagen = etASAPregunta.Texto0;
            }
            else
            {
                //mostramos el selector de imagenes
                ViewBag.ExisteImagen = false;
            }


            return View(setASAPregunta);
        }
        [HttpPost]
        public ActionResult ASAPreguntaEditar(long NroPregunta, ASAPreguntaSet setASAPreguntaForm, HttpPostedFileBase FileUpload)
        {
            long NumError = 0;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaEntity etASAPreguntaForm = setASAPreguntaForm.ltASAPregunta.First();
            if (etASAPreguntaForm != null)
            {
                ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ASAPreguntaEntity etASAPregunta = setASAPregunta.ltASAPregunta.First();
                etASAPregunta.Pregunta = etASAPreguntaForm.Pregunta;
                etASAPregunta.GrupoPregunta = etASAPreguntaForm.GrupoPregunta;
                etASAPregunta.UEstado = ASAPreguntaService.Estado.Modified;

                NumError = ASAPregunta.WebGuardar(setASAPregunta, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }
            else
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                string DirectorioArchivo = Server.MapPath("~/Uploads/Imagenes/ASAPregunta/");
                string[] allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "*.jpe", ".jpeg" };

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase upload = Request.Files[i]; //Uploaded file
                                                                  //Use the following properties to get file's name, size and MIMEType
                                                                  //int fileSize = file.ContentLength;
                                                                  //string fileName = file.FileName;
                                                                  //string mimeType = file.ContentType;
                                                                  //System.IO.Stream fileContent = file.InputStream;
                                                                  ////To save file, use SaveAs method
                                                                  //file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
                    if (upload != null && upload.ContentLength > 0)
                    {

                        var NombreArchivo = Path.GetFileNameWithoutExtension(upload.FileName);
                        var nombreValidado = NombreArchivo.Replace(" ", "_");
                        var ext = Path.GetExtension(upload.FileName);
                        var NombreCompleto = nombreValidado + DateTime.Now.Ticks + ext;
                        var path = Path.Combine(DirectorioArchivo, NombreCompleto);
                        if (allowedExtensions.Contains(ext))
                        {
                            if (!Directory.Exists(DirectorioArchivo))//Si no existe el folder lo creamos
                            {
                                Directory.CreateDirectory(DirectorioArchivo);
                            }

                            if (!System.IO.File.Exists(path)) // si no existe archivo en el folder
                            {
                                upload.SaveAs(path);
                                //Guardamos en la Configuracion
                                ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                                ASAPreguntaEntity etASAPregunta = setASAPregunta.ltASAPregunta.First();
                                etASAPregunta.Texto0 = NombreCompleto;
                                etASAPregunta.UEstado = ASAPreguntaService.Estado.Modified;
                                NumError = ASAPregunta.WebGuardar(setASAPregunta, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                            }
                        }
                    }
                }
            }

            if (NumError == 0)
            {
                return RedirectToAction("ASACuestionarioListaPregunta");
            }
            else
            {
                return RedirectToAction("ASACuestionarioListaPregunta");
            }
        }
        public ActionResult ASAPreguntaEliminar(long NroPregunta)
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaSet setASAPregunta = new ASAPreguntaSet();

            setASAPregunta = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            List<ASAGrupoPreguntaEntity> ltASAGrupoPregunta = ASAGrupoPregunta.WebASAGrupoPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltASAGrupoPregunta = ltASAGrupoPregunta.Select(s => s.GrupoPregunta).ToList();

            return View(setASAPregunta);
        }
        [HttpPost, ActionName("ASAPreguntaEliminar")]
        public ActionResult ASAPreguntaEliminarConfirmacion(long NroPregunta)
        {
            long NumError = 0;
            //string Grupo = string.Empty;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();

            if (NroPregunta != 0)
            {
                ASAPreguntaSet setASAPreguntaBorrar = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                NumError = ASAPregunta.WebEliminar(setASAPreguntaBorrar, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }
            else
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                return RedirectToAction("ASACuestionarioListaPregunta");
            }
            else
            {
                return RedirectToAction("ASACuestionarioListaPregunta");
            }
        }
        public ActionResult ASAPreguntaRespuestaNuevo(long NroPregunta)
        {
            ASAPreguntaRespuestaEntity etASAPreguntaRespuesta = new ASAPreguntaRespuestaEntity();
            etASAPreguntaRespuesta.NroPregunta = NroPregunta;
            etASAPreguntaRespuesta.Opcion = 0;
            etASAPreguntaRespuesta.Respuesta = string.Empty;
            etASAPreguntaRespuesta.Correcto = false;
            etASAPreguntaRespuesta.TipoPregunta = string.Empty;
            etASAPreguntaRespuesta.UEstado = ASAPreguntaService.Estado.Added;

            return PartialView(etASAPreguntaRespuesta);
        }
        [HttpPost]
        public ActionResult ASAPreguntaRespuestaNuevo(ASAPreguntaRespuestaEntity etASAPreguntaRespuesta)
        {
            long NumError = 0;
            ASAPreguntaRespuestaEntity etASAPreguntaRespuestaOriginal = etASAPreguntaRespuesta;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();

            if (etASAPreguntaRespuesta.Opcion >= 4 || etASAPreguntaRespuesta.Opcion <= 0)
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(etASAPreguntaRespuesta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etASAPreguntaRespuesta.Respuesta = etASAPreguntaRespuesta.Respuesta == null ? "" : etASAPreguntaRespuesta.Respuesta;
                etASAPreguntaRespuesta.TipoPregunta = etASAPreguntaRespuesta.TipoPregunta == null ? "" : etASAPreguntaRespuesta.TipoPregunta;
                etASAPreguntaRespuesta.UEstado = ASAPreguntaService.Estado.Added;
                setASAPregunta.ltASAPreguntaRespuesta.Add(etASAPreguntaRespuesta);

                NumError = ASAPregunta.WebGuardar(setASAPregunta, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("ASAPreguntaEditar", "ASACuestionario", new { NroPregunta = etASAPreguntaRespuesta.NroPregunta }) });
                //return PartialView("ASAPreguntaSuccess", new { SubCompania = etASAPreguntaRespuesta.SubCompania, Grupo = etASAPreguntaRespuesta.Grupo });
            }
            else
            {
                return PartialView(etASAPreguntaRespuestaOriginal);
            }

        }

        public ActionResult ASAPreguntaRespuestaEliminar(long NroPregunta, long Opcion)
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaRespuestaEntity etASAPreguntaRespuesta = new ASAPreguntaRespuestaEntity();

            ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etASAPreguntaRespuesta = setASAPregunta.ltASAPreguntaRespuesta.Where(x => x.NroPregunta == NroPregunta && x.Opcion == Opcion).First();
            //etASAPreguntaRespuesta = lt.Where(x => ).First();
            //etASAPreguntaRespuesta
            return PartialView(etASAPreguntaRespuesta);
        }

        [HttpPost]
        public ActionResult ASAPreguntaRespuestaEliminar(ASAPreguntaRespuestaEntity etASAPreguntaRespuesta)
        {
            long NumError = 0;
            ASAPreguntaRespuestaEntity etASAPreguntaRespuestaOriginal = etASAPreguntaRespuesta;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(etASAPreguntaRespuesta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etASAPreguntaRespuesta = setASAPregunta.ltASAPreguntaRespuesta.Where(x => x.NroPregunta == etASAPreguntaRespuesta.NroPregunta && x.Opcion == etASAPreguntaRespuesta.Opcion).First();
                etASAPreguntaRespuesta.UEstado = ASAPreguntaService.Estado.Deleted;
                setASAPregunta.ltASAPreguntaRespuestaEliminado.Add(etASAPreguntaRespuesta);

                NumError = ASAPregunta.WebGuardar(setASAPregunta, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("ASAPreguntaEditar", "ASACuestionario", new { NroPregunta = etASAPreguntaRespuesta.NroPregunta }) });
                //return PartialView("ASAPreguntaSuccess", new { SubCompania = etASAPreguntaRespuesta.SubCompania, Grupo = etASAPreguntaRespuesta.Grupo });
            }
            else
            {
                return PartialView(etASAPreguntaRespuestaOriginal);
            }

        }

        public ActionResult ASAPreguntaRespuestaEditar(long NroPregunta, long Opcion)
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASAPreguntaRespuestaEntity etASAPreguntaRespuesta = new ASAPreguntaRespuestaEntity();

            ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etASAPreguntaRespuesta = setASAPregunta.ltASAPreguntaRespuesta.Where(x => x.NroPregunta == NroPregunta && x.Opcion == Opcion).First();
            //etASAPreguntaRespuesta = lt.Where(x => ).First();
            //etASAPreguntaRespuesta
            return PartialView(etASAPreguntaRespuesta);
        }
        [HttpPost]
        public ActionResult ASAPreguntaRespuestaEditar(ASAPreguntaRespuestaEntity etASAPreguntaRespuesta)
        {
            long NumError = 0;
            ASAPreguntaRespuestaEntity etASAPreguntaRespuestaOriginal = etASAPreguntaRespuesta;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                ASAPreguntaSet setASAPregunta = ASAPregunta.WebSeleccionar(etASAPreguntaRespuesta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etASAPreguntaRespuesta.Respuesta = etASAPreguntaRespuesta.Respuesta == null ? "" : etASAPreguntaRespuesta.Respuesta;
                etASAPreguntaRespuesta.TipoPregunta = etASAPreguntaRespuesta.TipoPregunta == null ? "" : etASAPreguntaRespuesta.TipoPregunta;
                etASAPreguntaRespuesta.UEstado = ASAPreguntaService.Estado.Modified;
                setASAPregunta.ltASAPreguntaRespuesta.Add(etASAPreguntaRespuesta);

                NumError = ASAPregunta.WebGuardar(setASAPregunta, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("ASAPreguntaEditar", "ASACuestionario", new { NroPregunta = etASAPreguntaRespuesta.NroPregunta }) });
                //return PartialView("ASAPreguntaSuccess", new { SubCompania = etASAPreguntaRespuesta.SubCompania, Grupo = etASAPreguntaRespuesta.Grupo });
            }
            else
            {
                return PartialView(etASAPreguntaRespuestaOriginal);
            }

        }
        [HttpGet]
        public ActionResult ASACuestionarioDetalleTransaccion(string EstudianteCI, long Intento)
        {
            ASATransaccionInterfaceClient ASATransaccion = new ASATransaccionInterfaceClient();
            List<ASATransaccionEntity> ltASATransaccion = ASATransaccion.WebASATransaccionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltASATransaccion = ltASATransaccion.Where(x => x.Intento == Intento && x.EstudianteCI == EstudianteCI).ToList();

            return PartialView(ltASATransaccion);
        }
        [HttpGet]
        public ActionResult ASACuestionarioGenerarExamen()
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult ASACuestionarioGenerarExamen(string Grupo, int NumPreguntas = 0)
        {
            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASATReportePreguntaInterfaceClient ASATReportePregunta = new ASATReportePreguntaInterfaceClient();
            Random Random = new Random();

            MapPath = "~/Reports/ASACuestionario/ASACuestionarioGenerarExamen.rpt";
            FileDownloadName = "PreguntasASA" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";

            if (NumPreguntas != 0 && Grupo != "")
            {

                try
                {
                    //MapPath = "~/Reports/CDComanda/CDTTransaccion.rpt";
                    //FileDownloadName = "CDTTransaccion" + DateTime.Now + ".xls";

                    ReportDocument report = new ReportDocument();
                    List<ASATReportePreguntaEntity> ltASATReportePregunta = new List<ASATReportePreguntaEntity>();
                    List<ASAPreguntaSet> ltsetASAPregunta = new List<ASAPreguntaSet>();
                    List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(NumPreguntas).ToList();

                    //List<CDTTransaccionEntity> ltCDTTransaccion = ConsultaCDTTransaccion(ID_TRA19Ini, ID_TRA19Fin, ArticuloIni, ArticuloFin, DestinoImpresionIni, DestinoImpresionFin, FechaDocIni, FechaDocFin, UsuarioIni, UsuarioFin);
                    foreach (var item in ltASAPregunta)
                    {
                        ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(item.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                    }

                    bool RespCorrecta = false;
                    foreach (var setASAPregunta in ltsetASAPregunta)
                    {
                        foreach (var etASAPregunta in setASAPregunta.ltASAPregunta)
                        {
                            foreach (var etASAPreguntaRespuesta in setASAPregunta.ltASAPreguntaRespuesta)
                            {
                                if (etASAPreguntaRespuesta.Correcto)
                                {
                                    RespCorrecta = true;
                                }
                                else
                                {
                                    RespCorrecta = false;
                                }

                                ltASATReportePregunta.Add(new ASATReportePreguntaEntity
                                {
                                    SubCompania = (string)Session["SesionSubCompania"],
                                    Grupo = Grupo,
                                    NroPregunta = etASAPregunta.NroPregunta,
                                    Opcion = etASAPreguntaRespuesta.Opcion,
                                    Respuesta = etASAPreguntaRespuesta.Respuesta,
                                    PreguntaDescripcion = etASAPregunta.Pregunta,
                                    GrupoPregunta = etASAPregunta.GrupoPregunta,
                                    RespuestaCorrecta = RespCorrecta
                                });
                            }

                        }
                    }

                    report.Load(Server.MapPath(MapPath));
                    report.SetDataSource(ltASATReportePregunta);
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
                        //Guardamos la lista en el servidor
                        NumError = ASATReportePregunta.WebGuardarASATReportePregunta(ltASATReportePregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    }

                    if (NumError == 0)
                    {
                        return File(stream, ContentType, FileDownloadName);
                    }
                    else
                    {
                        return RedirectToAction("ASACuestionarioGenerarExamen");
                    }



                }
                catch (Exception e)
                {
                    return RedirectToAction("ASACuestionarioGenerarExamen");
                }

            }
            else
            {
                return RedirectToAction("ASACuestionarioGenerarExamen");
            }

        }
        public ActionResult ASAPreguntaBancoPregunta()
        {
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.NumPreguntas = ltASAPregunta.Count;

            return View(ltASAPregunta);
        }
        [HttpGet]
        public ActionResult ASACuestionarioHistorialExamen()
        {
            List<ASATReportePreguntaEntity> ltASATReportePreguntaDist = new List<ASATReportePreguntaEntity>();
            ASATReportePreguntaInterfaceClient ASATReportePregunta = new ASATReportePreguntaInterfaceClient();
            List<ASATReportePreguntaEntity> ltASATReportePregunta = ASATReportePregunta.WebASATReportePreguntaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            var ltDistinct = ltASATReportePregunta.Select(s => new { s.Grupo, s.FechaDoc }).Distinct().ToList();

            foreach (var item in ltDistinct)
            {
                ltASATReportePreguntaDist.Add(new ASATReportePreguntaEntity
                {
                    Grupo = item.Grupo,
                    FechaDoc = item.FechaDoc
                });
            }

            return View(ltASATReportePreguntaDist);
        }

        public ActionResult ASACuestionarioHistorialDescargar(string Grupo, DateTime FechaDoc)
        {
            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            ASATReportePreguntaInterfaceClient ASATReportePregunta = new ASATReportePreguntaInterfaceClient();

            MapPath = "~/Reports/ASACuestionario/ASACuestionarioGenerarExamen.rpt";
            FileDownloadName = "PreguntasASA" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";


            try
            {
                //MapPath = "~/Reports/CDComanda/CDTTransaccion.rpt";
                //FileDownloadName = "CDTTransaccion" + DateTime.Now + ".xls";

                ReportDocument report = new ReportDocument();
                List<ASATReportePreguntaEntity> ltASATReportePregunta = new List<ASATReportePreguntaEntity>();
                ltASATReportePregunta = ASATReportePregunta.WebASATReportePreguntaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltASATReportePregunta = ltASATReportePregunta.Where(x => x.Grupo == Grupo && x.FechaDoc.Day == FechaDoc.Day && x.FechaDoc.Month == FechaDoc.Month && x.FechaDoc.Year == FechaDoc.Year && x.FechaDoc.Hour == FechaDoc.Hour && x.FechaDoc.Minute == FechaDoc.Minute && x.FechaDoc.Second == FechaDoc.Second).ToList();

                report.Load(Server.MapPath(MapPath));
                report.SetDataSource(ltASATReportePregunta);
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
                    return RedirectToAction("ASACuestionarioHistorialExamen");
                }



            }
            catch (Exception e)
            {
                return RedirectToAction("ASACuestionarioHistorialExamen");
            }


        }
    }
}