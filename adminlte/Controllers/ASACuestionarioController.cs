using adminlte.ASAPreguntaService;
using adminlte.ASATransaccionService;
using adminlte.ASATRespuestaService;
using adminlte.AXFSesionService;
using adminlte.AXFUsuarioService;
using adminlte.Models;
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
            SelectList ltSelect = new SelectList(
            new List<ListItem>
            {
                new ListItem { Text = "Preguntas ASA" , Value = "ASACuestionarioEntrada" }
            }, "Value", "Text");

            ViewBag.ltSelect = ltSelect;

            return View();
        }

        [HttpPost]
        public ActionResult ASACuestionarioInicio(string Cuestionario)
        {
            if (Cuestionario != "")
            {
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
                    ViewBag.TiempoRestante = ASATRespuesta.WebASATRespuestaTiempoRestante((string)Session["Usuario"],60, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
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
                            var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                            if (RespGuardado == 0)
                            {
                                ltASAPregunta = (from Pregunta in ltASAPregunta
                                                 orderby Pregunta.NroPregunta ascending
                                                 select Pregunta).ToList();
                                foreach (ASAPreguntaEntity etASAPregunta in ltASAPregunta)
                                {
                                    ltsetASAPregunta.Add(ASAPregunta.WebSeleccionar(etASAPregunta.NroPregunta,(string)Session["Sesion"], (string)Session["SesionSubCompania"]));
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

                if (ltASAPregunta.Count >= count)
                {
                    ltASAPregunta = ltASAPregunta.OrderByDescending(x => Random.Next()).Take(count).ToList();
                    var RespGuardado = ASAPregunta.WebGuardarCuestionarioIni(ltASAPregunta, (string)Session["Usuario"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
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

            return Json(new { success = true  });
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


            return Json(new { success = true , url = Url.Action("Index", "Index") });
        }

        [HttpGet]
        public ActionResult ASACuestionarioExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ASACuestionarioExcel(HttpPostedFileBase FileUpload)
        {
            long NumError = 0;
            if (FileUpload != null && FileUpload.ContentLength > 0)
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
                            NumError = AgregrarExcelSet(dsResult.Tables[0]);
                        }
                    }
                }

            }

            if (NumError == 0)
            {
                return RedirectToAction("Index","Index");
            }
            else
            {
                return View();
            }

        }

        private long AgregrarExcelSet(DataTable dtExcel)
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
                    NumError = ASAPregunta.WebEliminarTodos((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    if(NumError == 0)
                    {
                        foreach (DataRow dtrow in dtExcel.Rows)
                        {
                            setASAPregunta = ASAPregunta.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                            long.TryParse(dtrow.ItemArray[0].ToString(), out NroPregunta);
                            foreach (ASAPreguntaEntity etASAPregunta in setASAPregunta.ltASAPregunta)
                            {
                                etASAPregunta.NroPregunta = NroPregunta;
                                etASAPregunta.Pregunta = dtrow.ItemArray[1].ToString();
                                etASAPregunta.UEstado = ASAPreguntaService.Estado.Modified;
                            }

                            List<ASAPreguntaRespuestaEntity> etASAPreguntaRespuesta = setASAPregunta.ltASAPreguntaRespuesta;
                            long Opcion = 1;
                            CorrectoR1 = dtrow.ItemArray[5].ToString() == "0" ? false : true;

                            etASAPreguntaRespuesta.Add(new ASAPreguntaRespuestaEntity {
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
            
            List<ASATransaccionEntity> ltASATransaccion = ASATransaccion.WebASATransaccionSeleccionarXSubCompania((string)Session["SesionSubCompania"],(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
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
                return RedirectToAction("ASACuestionarioGrafica", "ASACuestionario", new { Estudiante  = Estudiante });
            }
            else
            {
                return RedirectToAction("ASACuestionarioGraficaEstudiante", "ASACuestionario");
            }
        }
    }
}