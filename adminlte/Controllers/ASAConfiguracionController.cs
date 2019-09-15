using adminlte.ASAConfiguracionService;
using adminlte.TASGrupoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class ASAConfiguracionController : BaseController
    {
        // GET: ASAConfiguracion
        public ActionResult ASAConfiguracionLista()
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionSet setASAConfiguracion = new ASAConfiguracionSet();
            List<ASAConfiguracionEntity> ltASAConfiguracion = ASAConfiguracion.WebASAConfiguracionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            

            return View(ltASAConfiguracion);
        }

        [HttpGet]
        public ActionResult ASAConfiguracionNuevo(string MensajeError = "")
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            List<ASAConfiguracionEntity> ltASAConfiguracion = ASAConfiguracion.WebASAConfiguracionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();
            ViewBag.ltTASGrupo = ltTASGrupo.Where(ds => !ltASAConfiguracion.Any(db => db.Grupo == ds.Grupo)).Select(s => s.Grupo).ToList();
            ViewBag.MensajeError = MensajeError;

            return View(setASAConfiguracion);
        }

        [HttpPost]
        public ActionResult ASAConfiguracionNuevo(ASAConfiguracionSet setASAConfiguracion)
        {
            long NumError = 0;
            string Grupo = string.Empty;
            string MensajeError = string.Empty;

            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();

            if (setASAConfiguracion.ltASAConfiguracion != null)
            {
                ASAConfiguracionEntity etASAConfiguracion = setASAConfiguracion.ltASAConfiguracion.First();
                if (etASAConfiguracion.Grupo != null && etASAConfiguracion.Grupo != "")
                {
                    ASAConfiguracionSet setASAConfiguracionNuevo = ASAConfiguracion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    ASAConfiguracionEntity etASAConfiguracionNuevo = setASAConfiguracionNuevo.ltASAConfiguracion.First();
                    etASAConfiguracionNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etASAConfiguracionNuevo.Grupo = etASAConfiguracion.Grupo;
                    etASAConfiguracionNuevo.UEstado = ASAConfiguracionService.Estado.Added;
                    Grupo = etASAConfiguracion.Grupo;
                    NumError = ASAConfiguracion.WebGuardar(setASAConfiguracionNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                }
                else
                {
                    NumError = 1;
                    MensajeError = "Debe seleccionar un Grupo.";
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("ASAConfiguracionEditar", new { SubCompania = (string)Session["SesionSubCompania"], Grupo = Grupo });
            }
            else
            {
                return RedirectToAction("ASAConfiguracionNuevo", new { MensajeError = MensajeError });
            }

        }

        public ActionResult ASAConfiguracionEditar(string SubCompania, string Grupo)
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionSet setASAConfiguracion = new ASAConfiguracionSet();

            setASAConfiguracion = ASAConfiguracion.WebSeleccionar(SubCompania, Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(setASAConfiguracion);
        }

        public ActionResult ASAConfiguracionLineaNuevo(string SubCompania, string Grupo, string MensajeError = "" )
        {
            ASAConfiguracionLineaEntity etASAConfiguracionLinea = new ASAConfiguracionLineaEntity();
            etASAConfiguracionLinea.SubCompania = SubCompania;
            etASAConfiguracionLinea.Grupo = Grupo;
            etASAConfiguracionLinea.FechaDoc = DateTime.Today;
            etASAConfiguracionLinea.CantidadPregunta = 0;
            etASAConfiguracionLinea.FechaIni = DateTime.Today;
            etASAConfiguracionLinea.FechaFin = DateTime.Today;
            etASAConfiguracionLinea.UEstado = ASAConfiguracionService.Estado.Added;

            ViewBag.MensajeError = MensajeError;
            return PartialView(etASAConfiguracionLinea);
        }

        [HttpPost]
        public ActionResult ASAConfiguracionLineaNuevo(ASAConfiguracionLineaEntity etASAConfiguracionLinea)
        {
            long NumError = 0;
            ASAConfiguracionLineaEntity etASAConfiguracionLineaOriginal = etASAConfiguracionLinea;
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            bool EsMediaNoche = false;
            string MensajeError = string.Empty;

            if (etASAConfiguracionLinea.FechaIni.TimeOfDay >= etASAConfiguracionLinea.FechaFin.TimeOfDay)
            {
                if (etASAConfiguracionLinea.FechaFin.TimeOfDay == DateTime.Today.TimeOfDay)
                {
                    //significa q es media noche
                    if (etASAConfiguracionLinea.FechaIni >= etASAConfiguracionLinea.FechaFin.AddDays(1))
                    {
                        NumError = 1;
                        MensajeError += "";
                    }
                    else
                    {
                        EsMediaNoche = true;
                    }
                }
                else
                {
                    NumError = 1;
                    MensajeError += "La fecha inicial es mayor a la fecha final.";
                }
                
            }

            if (etASAConfiguracionLinea.CantidadPregunta <= 0)
            {
                NumError = 1;
                MensajeError += "La Cantidad de Preguntas no puede ser menor a 0.";
            }

            if (NumError == 0)
            {
                //agregamos al set el detalle
                ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(etASAConfiguracionLinea.SubCompania, etASAConfiguracionLinea.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etASAConfiguracionLinea.FechaDoc = etASAConfiguracionLinea.FechaDoc.Date + DateTime.Now.TimeOfDay;
                etASAConfiguracionLinea.FechaIni = etASAConfiguracionLinea.FechaDoc.Date + etASAConfiguracionLinea.FechaIni.TimeOfDay;
                if (EsMediaNoche)
                {
                    etASAConfiguracionLinea.FechaFin = etASAConfiguracionLinea.FechaDoc.AddDays(1).Date + etASAConfiguracionLinea.FechaFin.TimeOfDay;
                }
                else
                {
                    etASAConfiguracionLinea.FechaFin = etASAConfiguracionLinea.FechaDoc.Date + etASAConfiguracionLinea.FechaFin.TimeOfDay;
                }
                
                etASAConfiguracionLinea.UEstado = ASAConfiguracionService.Estado.Added;
                setASAConfiguracion.ltASAConfiguracionLinea.Add(etASAConfiguracionLinea);

                NumError = ASAConfiguracion.WebGuardar(setASAConfiguracion, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (NumError != 0)
                {
                    MensajeError += "Ocurrio un error al intentar guardar. Intente nuevamente.";
                }
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("ASAConfiguracionEditar", "ASAConfiguracion", new { SubCompania = etASAConfiguracionLinea.SubCompania, Grupo = etASAConfiguracionLinea.Grupo }) });
                //return PartialView("ASAConfiguracionSuccess", new { SubCompania = etASAConfiguracionLinea.SubCompania, Grupo = etASAConfiguracionLinea.Grupo });
            }
            else
            {
                ViewBag.MensajeError = MensajeError;
                return PartialView(etASAConfiguracionLineaOriginal);
            }

        }

        public ActionResult ASAConfiguracionLineaEliminar(string SubCompania, string Grupo, DateTime FechaDoc)
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionLineaEntity etASAConfiguracionLinea = new ASAConfiguracionLineaEntity();

            ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(SubCompania, Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etASAConfiguracionLinea = setASAConfiguracion.ltASAConfiguracionLinea.Where(x => x.SubCompania == SubCompania && x.Grupo == Grupo && x.FechaDoc.Day == FechaDoc.Day && x.FechaDoc.Month == FechaDoc.Month && x.FechaDoc.Year == FechaDoc.Year && x.FechaDoc.Hour == FechaDoc.Hour && x.FechaDoc.Minute == FechaDoc.Minute && x.FechaDoc.Second == FechaDoc.Second).First();
            //etASAConfiguracionLinea = lt.Where(x => ).First();
            //etASAConfiguracionLinea
            return PartialView(etASAConfiguracionLinea);
        }

        [HttpPost]
        public ActionResult ASAConfiguracionLineaEliminar(ASAConfiguracionLineaEntity etASAConfiguracionLinea)
        {
            long NumError = 0;
            ASAConfiguracionLineaEntity etASAConfiguracionLineaOriginal = etASAConfiguracionLinea;
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();

            if (NumError == 0)
            {
                //agregamos al set el detalle
                ASAConfiguracionSet setASAConfiguracion = ASAConfiguracion.WebSeleccionar(etASAConfiguracionLinea.SubCompania, etASAConfiguracionLinea.Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etASAConfiguracionLinea = setASAConfiguracion.ltASAConfiguracionLinea.Where(x => x.SubCompania == etASAConfiguracionLinea.SubCompania && x.Grupo == etASAConfiguracionLinea.Grupo && x.FechaDoc.Day == etASAConfiguracionLinea.FechaDoc.Day && x.FechaDoc.Month == etASAConfiguracionLinea.FechaDoc.Month && x.FechaDoc.Year == etASAConfiguracionLinea.FechaDoc.Year && x.FechaDoc.Hour == etASAConfiguracionLinea.FechaDoc.Hour && x.FechaDoc.Minute == etASAConfiguracionLinea.FechaDoc.Minute && x.FechaDoc.Second == etASAConfiguracionLinea.FechaDoc.Second).First();
                etASAConfiguracionLinea.UEstado = ASAConfiguracionService.Estado.Deleted;
                setASAConfiguracion.ltASAConfiguracionLineaEliminado.Add(etASAConfiguracionLinea);

                NumError = ASAConfiguracion.WebGuardar(setASAConfiguracion, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("ASAConfiguracionEditar", "ASAConfiguracion", new { SubCompania = etASAConfiguracionLinea.SubCompania, Grupo = etASAConfiguracionLinea.Grupo }) });
                //return PartialView("ASAConfiguracionSuccess", new { SubCompania = etASAConfiguracionLinea.SubCompania, Grupo = etASAConfiguracionLinea.Grupo });
            }
            else
            {
                return PartialView(etASAConfiguracionLineaOriginal);
            }

        }

        public ActionResult ASAConfiguracionEliminar(string SubCompania, string Grupo)
        {
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
            ASAConfiguracionSet setASAConfiguracion = new ASAConfiguracionSet();

            setASAConfiguracion = ASAConfiguracion.WebSeleccionar(SubCompania, Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(setASAConfiguracion);
        }

        [HttpPost]
        public ActionResult ASAConfiguracionEliminar(string Grupo)
        {
            long NumError = 0;
            //string Grupo = string.Empty;
            ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();

            if (Grupo != null)
            {
                ASAConfiguracionSet setASAConfiguracionBorrar = ASAConfiguracion.WebSeleccionar((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                NumError = ASAConfiguracion.WebEliminar(setASAConfiguracionBorrar, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }
            else
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                return RedirectToAction("ASAConfiguracionLista");
            }
            else
            {
                return RedirectToAction("ASAConfiguracionLista");
            }
        }

        public ActionResult ASAConfiguracionSuccess(string SubCompania, string Grupo)
        {
            ViewBag.Mensaje = "Accion completada exitosamente.";
            ViewBag.SubCompania = SubCompania;
            ViewBag.Grupo = Grupo;

            return PartialView();
        }
        //[HttpPost]
        //public ActionResult ASAConfiguracionEditar(ASAConfiguracionSet setASAConfiguracion, DateTime? FechaInicial)
        //{
        //    ASAConfiguracionInterfaceClient ASAConfiguracion = new ASAConfiguracionInterfaceClient();
        //    ASAConfiguracionEntity etASAConfiguracion = setASAConfiguracion.ltASAConfiguracion.First();
        //    DateTime FechaIni = FechaInicial.Value;

        //    List<ASAConfiguracionEntity> ltASAConfiguracion = ASAConfiguracion.WebASAConfiguracionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //    if (ltASAConfiguracion.Count > 0)
        //    {
        //        ASAConfiguracionSet setASAConfiguracionAntiguo = ASAConfiguracion.WebSeleccionar((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //        ASAConfiguracionEntity etASAConfiguracionAntiguo = setASAConfiguracionAntiguo.ltASAConfiguracion.First();
        //        etASAConfiguracionAntiguo.SubCompania = (string)Session["SesionSubCompania"];
        //        etASAConfiguracionAntiguo.CantidadPregunta = etASAConfiguracion.CantidadPregunta == null ? 100 : etASAConfiguracion.CantidadPregunta;
        //        etASAConfiguracionAntiguo.FechaIni = FechaIni == null ? DateTime.Now : FechaIni.Date + etASAConfiguracion.FechaIni.TimeOfDay;
        //        etASAConfiguracionAntiguo.FechaFin = FechaIni == null ? DateTime.Now.AddHours(1) : FechaIni.Date + etASAConfiguracion.FechaFin.TimeOfDay;
        //        etASAConfiguracionAntiguo.UEstado = Estado.Modified;
        //        long NumError = ASAConfiguracion.WebGuardar(setASAConfiguracionAntiguo,false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

        //    }
        //    else
        //    {
        //        ASAConfiguracionSet setASAConfiguracionNuevo = ASAConfiguracion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
        //        ASAConfiguracionEntity etASAConfiguracionNuevo = setASAConfiguracionNuevo.ltASAConfiguracion.First();
        //        etASAConfiguracionNuevo.SubCompania = (string)Session["SesionSubCompania"];
        //        etASAConfiguracionNuevo.CantidadPregunta = etASAConfiguracion.CantidadPregunta == null ? 100 : etASAConfiguracion.CantidadPregunta;
        //        etASAConfiguracionNuevo.FechaIni = FechaIni == null ? DateTime.Now : FechaIni.Date + etASAConfiguracion.FechaIni.TimeOfDay;
        //        etASAConfiguracionNuevo.FechaFin = FechaIni == null ? DateTime.Now.AddHours(1) : FechaIni.Date + etASAConfiguracion.FechaFin.TimeOfDay;
        //        etASAConfiguracionNuevo.UEstado = Estado.Added;
        //        long NumError = ASAConfiguracion.WebGuardar(setASAConfiguracionNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

        //    }

        //    return RedirectToAction("Index", "Index");
        //}

    }
}