using adminlte.ASAConfiguracionService;
using adminlte.ASAGrupoPreguntaService;
using adminlte.ASAPreguntaService;
using adminlte.ASATransaccionService;
using adminlte.ASATRespuestaService;
using adminlte.AXFSesionService;
using adminlte.AXFUsuarioService;
using adminlte.Models;
using adminlte.TASEstudianteService;
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
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            TASEstudianteSet setTASEstudiante = TASEstudiante.WebSeleccionar((string)Session["SesionSubCompania"],(string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if(setTASEstudiante.ltTASEstudiante.Count > 0)
            {
                //si existe el estudiante
                TASEstudianteEntity etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();
                string Grupo = etTASEstudiante.Grupo;
                //verificamos si existe algun grupo activo en la configuracion del asa
                ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(etTASEstudiante.SubCompania, etTASEstudiante.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                DateTime dtHoraServidor = AXFSesion.WebObtenerHoraServidor();
                foreach (var item in setASAConfiguracion.ltASAConfiguracionLinea)
                {
                    if(item.FechaDoc.Date == dtHoraServidor.Date)
                    {
                        //si son mismas fechas comparar el rango de las horas
                        if (item.FechaIni.Hour >= dtHoraServidor.Hour && dtHoraServidor.Hour <= item.FechaFin.Hour)
                        {
                            ExisteASA = true;
                        }
                    }
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
            }
            else
            {
                ViewBag.ltSelect = ltSelectPractica;
                ViewBag.ExisteASA = false;
            }
            

            return View();
        }

        [HttpPost]
        public ActionResult ASACuestionarioInicio(string Cuestionario, List<string> GrupoPregunta)
        {
            if (Cuestionario == "ASACuestionarioExamen")
            {
                return RedirectToAction(Cuestionario, "ASACuestionario");
            }
            else if(Cuestionario == "ASACuestionarioEntrada" && GrupoPregunta != null)
            {
                TempData["GrupoPregunta"] = GrupoPregunta;
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
            List<string> GrupoPregunta = (List<string>)TempData["GrupoPregunta"];
            //UPDATE METODO WebASATRespuestaExiste
            //UPDATE METODO WebASATRespuestaSeleccionarSiCumpleHorario
            //UPDATE METODO WebASAPreguntaSeleccionarTodo
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            Random Random = new Random();
            int count = 100;
            List<ASAPreguntaSet> ltsetASAPregunta = new List<ASAPreguntaSet>();
            //Verificamos si ya abrio una pregunta ASA, 
            //Si se abrio anteriormente cuestionario verificar primero la hora, si aun esta dentro el horario recuperar y llenar formulario con anteriores preguntas ,fuera de horario cerrar y empezar otro
            //NO se abrio entonces inicialimos un nuevo cuestionario con las opciones en 0

            bool ExisteCuestionarioPrevio = ASATRespuesta.WebASATRespuestaExiste((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (ExisteCuestionarioPrevio)
            {
                //Obtenemos el datetime del servidor
                bool SiCumpleHorario = ASATRespuesta.WebASATRespuestaSiCumpleHorario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (SiCumpleHorario)
                {
                    ltsetASAPregunta = ASAPregunta.WebASATRespuestaSeleccionarPreguntaXEstudiante((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    ViewBag.TiempoRestante = ASATRespuesta.WebASATRespuestaTiempoRestante((string)Session["Usuario"], 60, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
                else
                {
                    //cerramos el anterior
                    bool RespCerrado = ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (RespCerrado)
                    {
                        List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        ltASAPregunta = ltASAPregunta.Where(x => GrupoPregunta.Contains(x.GrupoPregunta)).ToList();

                        if (ltASAPregunta.Count >= count)
                        {
                            ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(count).ToList();
                            var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta,false, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                            if (RespGuardado == 0)
                            {
                                ltASAPregunta = (from Pregunta in ltASAPregunta
                                                 orderby Pregunta.NroPregunta ascending
                                                 select Pregunta).ToList();
                                foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                                {
                                    ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                                }
                                ViewBag.TiempoRestante = 60;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltASAPregunta = ltASAPregunta.Where(x => GrupoPregunta.Contains(x.GrupoPregunta)).ToList();

                if (ltASAPregunta.Count >= count)
                {
                    
                    ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(count).ToList();
                    var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta,false, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (RespGuardado == 0)
                    {
                        ltASAPregunta = (from Pregunta in ltASAPregunta
                                         orderby Pregunta.NroPregunta ascending
                                         select Pregunta).ToList();
                        foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                        {
                            ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                        }
                        ViewBag.TiempoRestante = 60;
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
            long TiempoMaximo = 0;
            long TiempoRestante = 0;
            DateTime FechaIni = DateTime.Now;
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            ASAGrupoPreguntaInterfaceClient ASAGrupoPregunta = new ASAGrupoPreguntaInterfaceClient();
            TASEstudianteSet setTASEstudiante = TASEstudiante.WebSeleccionar((string)Session["SesionSubCompania"], (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (setTASEstudiante.ltTASEstudiante.Count > 0)
            {
                //si existe el estudiante
                TASEstudianteEntity etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();
                string Grupo = etTASEstudiante.Grupo;
                //verificamos si existe algun grupo activo en la configuracion del asa
                ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(etTASEstudiante.SubCompania, etTASEstudiante.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                DateTime dtHoraServidor = AXFSesion.WebObtenerHoraServidor();
                foreach (var item in setASAConfiguracion.ltASAConfiguracionLinea)
                {
                    if (item.FechaDoc.Date == dtHoraServidor.Date)
                    {
                        //si son mismas fechas comparar el rango de las horas
                        if (item.FechaIni.Hour >= dtHoraServidor.Hour && dtHoraServidor.Hour <= item.FechaFin.Hour)
                        {
                            double Minutos = (item.FechaFin - item.FechaIni).TotalMinutes;
                            TiempoMaximo = Convert.ToInt64(Minutos);

                            double MinutosRestante = (dtHoraServidor - item.FechaIni).TotalMinutes;
                            TiempoRestante = TiempoMaximo - Convert.ToInt64(MinutosRestante);

                            FechaIni = item.FechaIni;
                        }
                    }
                }
            }

            //UPDATE METODO WebASATRespuestaExiste
            //UPDATE METODO WebASATRespuestaSeleccionarSiCumpleHorario
            //UPDATE METODO WebASAPreguntaSeleccionarTodo
            ASAPreguntaInterfaceClient ASAPregunta = new ASAPreguntaInterfaceClient();
            ASATRespuestaInterfaceClient ASATRespuesta = new ASATRespuestaInterfaceClient();
            Random Random = new Random();
            int count = 100;
            List<ASAPreguntaSet> ltsetASAPregunta = new List<ASAPreguntaSet>();

            //Verificamos si ya abrio una pregunta ASA, 
            //Si se abrio anteriormente cuestionario verificar primero la hora, si aun esta dentro el horario recuperar y llenar formulario con anteriores preguntas ,fuera de horario cerrar y empezar otro
            //NO se abrio entonces inicialimos un nuevo cuestionario con las opciones en 0

            bool ExisteCuestionarioPrevio = ASATRespuesta.WebASATRespuestaExiste((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            if (ExisteCuestionarioPrevio)
            {
                //Obtenemos el datetime del servidor
                bool Menor= ASATRespuesta.WebASATRespuestaMenorFecha((string)Session["Usuario"], FechaIni,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (!Menor)
                {
                    ltsetASAPregunta = ASAPregunta.WebASATRespuestaSeleccionarPreguntaXEstudiante((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    ViewBag.TiempoRestante = ASATRespuesta.WebASATRespuestaTiempoRestante((string)Session["Usuario"], TiempoMaximo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
                else
                {
                    //cerramos el anterior
                    bool RespCerrado = ASATRespuesta.WebASATRespuestaCerrarCuestionario((string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (RespCerrado)
                    {
                        List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                        
                        if (ltASAPregunta.Count >= count)
                        {
                            ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(count).ToList();
                            var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta, true,(string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                            if (RespGuardado == 0)
                            {
                                ltASAPregunta = (from Pregunta in ltASAPregunta
                                                 orderby Pregunta.NroPregunta ascending
                                                 select Pregunta).ToList();
                                foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                                {
                                    ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                                }

                                ViewBag.TiempoRestante = TiempoRestante;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ASAPreguntaEntity> ltASAPregunta = ASAPregunta.WebASAPreguntaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                
                if (ltASAPregunta.Count >= count)
                {

                    ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(count).ToList();
                    var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta,true, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if (RespGuardado == 0)
                    {
                        ltASAPregunta = (from Pregunta in ltASAPregunta
                                         orderby Pregunta.NroPregunta ascending
                                         select Pregunta).ToList();
                        foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                        {
                            ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta, (string)Session["Sesion"], (string)Session["SesionSubCompania"]));
                        }
                        ViewBag.TiempoRestante = TiempoRestante;
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
            var ltAgrupada = ltASATransaccion.Where(x => x.EstudianteCI == Estudiante).GroupBy(g => g.Intento).ToList();
            
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
                return RedirectToAction("ASAPreguntaEditar", new {NroPregunta = NroPregunta });
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

            return View(setASAPregunta);
        }
        [HttpPost]
        public ActionResult ASAPreguntaEditar(long NroPregunta, ASAPreguntaSet setASAPreguntaForm)
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


    }
}