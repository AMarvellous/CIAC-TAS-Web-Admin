using adminlte.TASEstudianteMateriasAsignacionService;
using adminlte.TASEstudianteService;
using adminlte.TASGrupoService;
using adminlte.TASMateriaService;
using adminlte.TASProgramaService;
using adminlte.TASRegistroNotasService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace adminlte.Controllers
{
    public class TASEstudianteController : BaseController
    {
        // GET: TASEstudiante
        public ActionResult TASEstudianteLista()
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            var ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASEstudiante);
        }

        [HttpGet]
        public ActionResult TASEstudianteNuevo(string MensajeError = "")
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            TASEstudianteSet setTASEstudiante = TASEstudiante.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            List<TASProgramaEntity> ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASPrograma = ltTASPrograma.Select(s => s.Programa).ToList();

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            var ltSexo = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Femenino", Value = "F" },
                        new ListItem { Text = "Masculino", Value = "M"},
                   }, "Value", "Text");
            ViewBag.ltSexo = ltSexo;

            var ltVacunaAntitetanica = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "En Progreso", Value = "1" },
                        new ListItem { Text = "Completa", Value = "2"},
                   }, "Value", "Text");
            ViewBag.ltVacunaAntitetanica = ltVacunaAntitetanica;

            var ltExamenPsifisiologico = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Habilitada", Value = "1" },
                        new ListItem { Text = "No Habilitada", Value = "2"},
                   }, "Value", "Text");
            ViewBag.ltExamenPsifisiologico = ltExamenPsifisiologico;

            var ltSiNo = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Si", Value = "1" },
                        new ListItem { Text = "No", Value = "2"},
                   }, "Value", "Text");
            ViewBag.ltSiNo = ltSiNo;

            ViewBag.MensajeError = MensajeError;

            return View(setTASEstudiante);
        }

        [HttpPost]
        public ActionResult TASEstudianteNuevo(TASEstudianteSet setTASEstudiante, 
            string Sexo, string VacunaAntitetanica, string ExamenPsicofisiologico, string InstruccionPrevia, string ExperienciaPrevia)
        {
            long NumError = 0;
            string EstudianteCI = string.Empty;
            string MensajeError = string.Empty;

            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();

            if (setTASEstudiante.ltTASEstudiante != null)
            {
                TASEstudianteEntity etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();
                if (etTASEstudiante.Grupo == null || etTASEstudiante.Grupo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe seleccionar un Grupo. ";
                }
                
                if(etTASEstudiante.Programa == null || etTASEstudiante.Programa == "")
                {
                    NumError = 1;
                    MensajeError += "Debe seleccionar un Programa. ";
                }
                
                //if (etTASEstudiante.Sexo == null || etTASEstudiante.Sexo == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe seleccionar el campo Sexo. ";
                //}

                if (etTASEstudiante.EstudianteCI == null || etTASEstudiante.EstudianteCI == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar el CI del estudiante. ";
                }

                //if (etTASEstudiante.EstadoCivil == null || etTASEstudiante.EstadoCivil == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe seleccionar el campo EstadoCivil.";
                //}


                //if (etTASEstudiante.Sexo == null || etTASEstudiante.Sexo == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe llenar el campo CodigoTAS.";
                //}

                if (VacunaAntitetanica == null || VacunaAntitetanica == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Vacuna Antitetanica.";
                }

                if (VacunaAntitetanica == null || VacunaAntitetanica == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Vacuna Antitetanica.";
                }

                if (ExamenPsicofisiologico == null || ExamenPsicofisiologico == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Examen Psicofisiologico.";
                }

                if (InstruccionPrevia == null || InstruccionPrevia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Instruccion Previa.";
                }

                if (ExperienciaPrevia == null || ExperienciaPrevia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Experiencia Previa.";
                }

                if (NumError == 0)
                {
                    TASEstudianteSet setTASEstudianteNuevo = TASEstudiante.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASEstudianteEntity etTASEstudianteNuevo = setTASEstudianteNuevo.ltTASEstudiante.First();
                    etTASEstudianteNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASEstudianteNuevo.EstudianteCI = etTASEstudiante.EstudianteCI == null ? "" : etTASEstudiante.EstudianteCI;
                    etTASEstudianteNuevo.Programa = etTASEstudiante.Programa == null ? "" : etTASEstudiante.Programa;
                    etTASEstudianteNuevo.Grupo = etTASEstudiante.Grupo == null ? "" : etTASEstudiante.Grupo;
                    //CodigoTAS Same as EstudianteCI
                    etTASEstudianteNuevo.CodigoTAS = etTASEstudiante.EstudianteCI == null ? "" : etTASEstudiante.EstudianteCI;
                    etTASEstudianteNuevo.Fecha = etTASEstudiante.Fecha;
                    etTASEstudianteNuevo.Nombre1 = etTASEstudiante.Nombre1 == null ? "" : etTASEstudiante.Nombre1;
                    etTASEstudianteNuevo.Nombre2 = etTASEstudiante.Nombre2 == null ? "" : etTASEstudiante.Nombre2;
                    etTASEstudianteNuevo.Apellido1 = etTASEstudiante.Apellido1 == null ? "" : etTASEstudiante.Apellido1;
                    etTASEstudianteNuevo.Apellido2 = etTASEstudiante.Apellido2 == null ? "" : etTASEstudiante.Apellido2;
                    etTASEstudianteNuevo.LugarNacimiento = etTASEstudiante.LugarNacimiento == null ? "" : etTASEstudiante.LugarNacimiento;
                    etTASEstudianteNuevo.Sexo = Sexo == null ? "" : Sexo;
                    etTASEstudianteNuevo.FechaNacimiento = etTASEstudiante.FechaNacimiento;
                    etTASEstudianteNuevo.Nacionalidad = etTASEstudiante.Nacionalidad == null ? "" : etTASEstudiante.Nacionalidad;
                    etTASEstudianteNuevo.EstadoCivil = etTASEstudiante.EstadoCivil == null ? "" : etTASEstudiante.EstadoCivil;
                    etTASEstudianteNuevo.Domicilio = etTASEstudiante.Domicilio == null ? "" : etTASEstudiante.Domicilio;
                    etTASEstudianteNuevo.Telefono = etTASEstudiante.Telefono == null ? "" : etTASEstudiante.Telefono;
                    etTASEstudianteNuevo.Celular = etTASEstudiante.Celular == null ? "" : etTASEstudiante.Celular;
                    etTASEstudianteNuevo.TelefonoFamiliar = etTASEstudiante.TelefonoFamiliar == null ? "" : etTASEstudiante.TelefonoFamiliar;
                    etTASEstudianteNuevo.Correo = etTASEstudiante.Correo == null ? "" : etTASEstudiante.Correo;
                    etTASEstudianteNuevo.NombrePadre = etTASEstudiante.NombrePadre == null ? "" : etTASEstudiante.NombrePadre;
                    etTASEstudianteNuevo.CelularPadre = etTASEstudiante.CelularPadre == null ? "" : etTASEstudiante.CelularPadre;
                    etTASEstudianteNuevo.NombreTutor = etTASEstudiante.NombreTutor == null ? "" : etTASEstudiante.NombreTutor;
                    etTASEstudianteNuevo.CelularTutor = etTASEstudiante.CelularTutor == null ? "" : etTASEstudiante.CelularTutor;

                    etTASEstudianteNuevo.VacunaAntitetanica = VacunaAntitetanica == "1" ? false : true;
                    etTASEstudianteNuevo.ExamenPsicofisiologico = ExamenPsicofisiologico == "2" ? false : true;
                    etTASEstudianteNuevo.CodigoSeguro = etTASEstudiante.CodigoSeguro == null ? "" : etTASEstudiante.CodigoSeguro;
                    etTASEstudianteNuevo.FechaSeguro = etTASEstudiante.FechaSeguro;
                    etTASEstudianteNuevo.InstruccionPrevia = InstruccionPrevia == "2" ? false : true;
                    etTASEstudianteNuevo.ExperienciaPrevia = ExperienciaPrevia == "2" ? false : true;
                    etTASEstudianteNuevo.Observaciones = etTASEstudiante.Observaciones == null ? "" : etTASEstudiante.Observaciones;

                    etTASEstudianteNuevo.UEstado = TASEstudianteService.Estado.Added;
                    EstudianteCI = etTASEstudiante.EstudianteCI;
                    NumError = TASEstudiante.WebGuardar(setTASEstudianteNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASEstudianteEditar", new { SubCompania = (string)Session["SesionSubCompania"], EstudianteCI = EstudianteCI });
            }
            else
            {
                TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
                List<TASProgramaEntity> ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASPrograma = ltTASPrograma.Select(s => s.Programa).ToList();

                TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
                List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

                var ltSexo = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "Femenino", Value = "F" },
                        new ListItem { Text = "Masculino", Value = "M"},
                       }, "Value", "Text");
                ViewBag.ltSexo = ltSexo;

                var ltVacunaAntitetanica = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "En Progreso", Value = "1" },
                        new ListItem { Text = "Completa", Value = "2"},
                       }, "Value", "Text");
                ViewBag.ltVacunaAntitetanica = ltVacunaAntitetanica;

                var ltExamenPsifisiologico = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "Habilitada", Value = "1" },
                        new ListItem { Text = "No Habilitada", Value = "2"},
                       }, "Value", "Text");
                ViewBag.ltExamenPsifisiologico = ltExamenPsifisiologico;

                var ltSiNo = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "Si", Value = "1" },
                        new ListItem { Text = "No", Value = "2"},
                       }, "Value", "Text");
                ViewBag.ltSiNo = ltSiNo;
                ViewBag.MensajeError = MensajeError;

                return View(setTASEstudiante);
            }

        }

        [HttpGet]
        public ActionResult TASEstudianteEditar(string SubCompania, string EstudianteCI, string MensajeError = "")
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            TASEstudianteSet setTASEstudiante = new TASEstudianteSet();

            setTASEstudiante = TASEstudiante.WebSeleccionar(SubCompania, EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            List<TASProgramaEntity> ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASPrograma = ltTASPrograma.Select(s => s.Programa).ToList();

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            ViewBag.ltSexo = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Femenino", Value = "F" , Selected = setTASEstudiante.ltTASEstudiante.First().Sexo == "F" ? true : false },
                        new ListItem { Text = "Masculino", Value = "M", Selected = setTASEstudiante.ltTASEstudiante.First().Sexo == "M" ? true : false},
                   }, "Value", "Text", setTASEstudiante.ltTASEstudiante.First().Sexo);;
            //ltSexo.Where(x => x.Value == setTASEstudiante.ltTASEstudiante.First().Sexo).First().Selected = true;
            //ViewBag.ltSexo = ltSexo;

            var valueVacunaAntitetanica = setTASEstudiante.ltTASEstudiante.First().VacunaAntitetanica ? "2" : "1";
            var ltVacunaAntitetanica = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "En Progreso", Value = "1", Selected = !setTASEstudiante.ltTASEstudiante.First().VacunaAntitetanica},
                        new ListItem { Text = "Completa", Value = "2", Selected = setTASEstudiante.ltTASEstudiante.First().VacunaAntitetanica},
                   }, "Value", "Text", valueVacunaAntitetanica);
            
            //var etVacunaAntitetanica = ltVacunaAntitetanica.Where(x => x.Value == valueVacunaAntitetanica).FirstOrDefault();
            //etVacunaAntitetanica.Selected = true;
            ViewBag.ltVacunaAntitetanica = ltVacunaAntitetanica;

            var valueExamenPsifisiologico = setTASEstudiante.ltTASEstudiante.First().ExamenPsicofisiologico ? "1" : "2";
            var ltExamenPsifisiologico = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Habilitada", Value = "1", Selected = setTASEstudiante.ltTASEstudiante.First().ExamenPsicofisiologico },
                        new ListItem { Text = "No Habilitada", Value = "2" , Selected = !setTASEstudiante.ltTASEstudiante.First().ExamenPsicofisiologico},
                   }, "Value", "Text", valueExamenPsifisiologico);
            //
            //var etExamenPsifisiologico = ltExamenPsifisiologico.Where(x => x.Value == valueExamenPsifisiologico).FirstOrDefault();
            //etExamenPsifisiologico.Selected = true;
            ViewBag.ltExamenPsifisiologico = ltExamenPsifisiologico;

            var valueInstruccionPrevia = setTASEstudiante.ltTASEstudiante.First().InstruccionPrevia ? "1" : "2";
            var ltSiNoInstruccionPrevia = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Si", Value = "1", Selected = setTASEstudiante.ltTASEstudiante.First().InstruccionPrevia },
                        new ListItem { Text = "No", Value = "2", Selected = !setTASEstudiante.ltTASEstudiante.First().InstruccionPrevia },
                   }, "Value", "Text", valueInstruccionPrevia);
            //
            //var etInstruccionPrevia = ltSiNoInstruccionPrevia.Where(x => x.Value == valueInstruccionPrevia).FirstOrDefault();
            //etInstruccionPrevia.Selected = true;
            ViewBag.ltSiNoInstruccionPrevia = ltSiNoInstruccionPrevia;

            var valueExperienciaPrevia = setTASEstudiante.ltTASEstudiante.First().ExperienciaPrevia ? "1" : "2";
            var ltSiNoExperienciaPrevia = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Si", Value = "1", Selected = setTASEstudiante.ltTASEstudiante.First().ExperienciaPrevia },
                        new ListItem { Text = "No", Value = "2", Selected = !setTASEstudiante.ltTASEstudiante.First().ExperienciaPrevia},
                   }, "Value", "Text", valueExperienciaPrevia);
            //
            //var etExperienciaPrevia = ltExamenPsifisiologico.Where(x => x.Value == valueExperienciaPrevia).FirstOrDefault();
            //etExperienciaPrevia.Selected = true;
            ViewBag.ltSiNoExperienciaPrevia = ltSiNoExperienciaPrevia;

            ViewBag.MensajeError = MensajeError;

            return View(setTASEstudiante);
        }

        [HttpPost]
        public ActionResult TASEstudianteEditar(TASEstudianteSet setTASEstudiante,
            string Sexo, string VacunaAntitetanica, string ExamenPsicofisiologico, string InstruccionPrevia, string ExperienciaPrevia)
        {
            long NumError = 0;
            string EstudianteCI = string.Empty;
            string MensajeError = string.Empty;

            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();

            if (setTASEstudiante.ltTASEstudiante != null)
            {
                TASEstudianteEntity etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();
                if (etTASEstudiante.Grupo == null || etTASEstudiante.Grupo == "")
                {
                    NumError = 1;
                    MensajeError += "Debe seleccionar un Grupo. ";
                }

                if (etTASEstudiante.Programa == null || etTASEstudiante.Programa == "")
                {
                    NumError = 1;
                    MensajeError += "Debe seleccionar un Programa. ";
                }

                //if (etTASEstudiante.Sexo == null || etTASEstudiante.Sexo == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe seleccionar el campo Sexo. ";
                //}

                if (etTASEstudiante.EstudianteCI == null || etTASEstudiante.EstudianteCI == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar el CI del estudiante. ";
                }

                //if (etTASEstudiante.EstadoCivil == null || etTASEstudiante.EstadoCivil == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe seleccionar el campo EstadoCivil.";
                //}


                //if (etTASEstudiante.Sexo == null || etTASEstudiante.Sexo == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe llenar el campo CodigoTAS.";
                //}

                if (VacunaAntitetanica == null || VacunaAntitetanica == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Vacuna Antitetanica.";
                }

                if (VacunaAntitetanica == null || VacunaAntitetanica == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Vacuna Antitetanica.";
                }

                if (ExamenPsicofisiologico == null || ExamenPsicofisiologico == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Examen Psicofisiologico.";
                }

                if (InstruccionPrevia == null || InstruccionPrevia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Instruccion Previa.";
                }

                if (ExperienciaPrevia == null || ExperienciaPrevia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe llenar el campo Experiencia Previa.";
                }

                if (NumError == 0)
                {
                    TASEstudianteSet setTASEstudianteNuevo = TASEstudiante.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASEstudianteEntity etTASEstudianteNuevo = setTASEstudianteNuevo.ltTASEstudiante.First();
                    etTASEstudianteNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASEstudianteNuevo.EstudianteCI = etTASEstudiante.EstudianteCI == null ? "" : etTASEstudiante.EstudianteCI;
                    etTASEstudianteNuevo.Programa = etTASEstudiante.Programa == null ? "" : etTASEstudiante.Programa;
                    etTASEstudianteNuevo.Grupo = etTASEstudiante.Grupo == null ? "" : etTASEstudiante.Grupo;
                    //CodigoTAS Same as EstudianteCI
                    etTASEstudianteNuevo.CodigoTAS = etTASEstudiante.EstudianteCI == null ? "" : etTASEstudiante.EstudianteCI;
                    etTASEstudianteNuevo.Fecha = etTASEstudiante.Fecha;
                    etTASEstudianteNuevo.Nombre1 = etTASEstudiante.Nombre1 == null ? "" : etTASEstudiante.Nombre1;
                    etTASEstudianteNuevo.Nombre2 = etTASEstudiante.Nombre2 == null ? "" : etTASEstudiante.Nombre2;
                    etTASEstudianteNuevo.Apellido1 = etTASEstudiante.Apellido1 == null ? "" : etTASEstudiante.Apellido1;
                    etTASEstudianteNuevo.Apellido2 = etTASEstudiante.Apellido2 == null ? "" : etTASEstudiante.Apellido2;
                    etTASEstudianteNuevo.LugarNacimiento = etTASEstudiante.LugarNacimiento == null ? "" : etTASEstudiante.LugarNacimiento;
                    etTASEstudianteNuevo.Sexo = Sexo == null ? "" : Sexo;
                    etTASEstudianteNuevo.FechaNacimiento = etTASEstudiante.FechaNacimiento;
                    etTASEstudianteNuevo.Nacionalidad = etTASEstudiante.Nacionalidad == null ? "" : etTASEstudiante.Nacionalidad;
                    etTASEstudianteNuevo.EstadoCivil = etTASEstudiante.EstadoCivil == null ? "" : etTASEstudiante.EstadoCivil;
                    etTASEstudianteNuevo.Domicilio = etTASEstudiante.Domicilio == null ? "" : etTASEstudiante.Domicilio;
                    etTASEstudianteNuevo.Telefono = etTASEstudiante.Telefono == null ? "" : etTASEstudiante.Telefono;
                    etTASEstudianteNuevo.Celular = etTASEstudiante.Celular == null ? "" : etTASEstudiante.Celular;
                    etTASEstudianteNuevo.TelefonoFamiliar = etTASEstudiante.TelefonoFamiliar == null ? "" : etTASEstudiante.TelefonoFamiliar;
                    etTASEstudianteNuevo.Correo = etTASEstudiante.Correo == null ? "" : etTASEstudiante.Correo;
                    etTASEstudianteNuevo.NombrePadre = etTASEstudiante.NombrePadre == null ? "" : etTASEstudiante.NombrePadre;
                    etTASEstudianteNuevo.CelularPadre = etTASEstudiante.CelularPadre == null ? "" : etTASEstudiante.CelularPadre;
                    etTASEstudianteNuevo.NombreTutor = etTASEstudiante.NombreTutor == null ? "" : etTASEstudiante.NombreTutor;
                    etTASEstudianteNuevo.CelularTutor = etTASEstudiante.CelularTutor == null ? "" : etTASEstudiante.CelularTutor;

                    etTASEstudianteNuevo.VacunaAntitetanica = VacunaAntitetanica == "1" ? false : true;
                    etTASEstudianteNuevo.ExamenPsicofisiologico = ExamenPsicofisiologico == "2" ? false : true;
                    etTASEstudianteNuevo.CodigoSeguro = etTASEstudiante.CodigoSeguro == null ? "" : etTASEstudiante.CodigoSeguro;
                    etTASEstudianteNuevo.FechaSeguro = etTASEstudiante.FechaSeguro;
                    etTASEstudianteNuevo.InstruccionPrevia = InstruccionPrevia == "2" ? false : true;
                    etTASEstudianteNuevo.ExperienciaPrevia = ExperienciaPrevia == "2" ? false : true;
                    etTASEstudianteNuevo.Observaciones = etTASEstudiante.Observaciones == null ? "" : etTASEstudiante.Observaciones;

                    etTASEstudianteNuevo.UEstado = TASEstudianteService.Estado.Modified;
                    EstudianteCI = etTASEstudiante.EstudianteCI;
                    NumError = TASEstudiante.WebGuardar(setTASEstudianteNuevo, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASEstudianteLista");
            }
            else
            {
                TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
                List<TASProgramaEntity> ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASPrograma = ltTASPrograma.Select(s => s.Programa).ToList();

                TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
                List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

                var ltSexo = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "Femenino", Value = "F" },
                        new ListItem { Text = "Masculino", Value = "M"},
                       }, "Value", "Text");
                ViewBag.ltSexo = ltSexo;

                var ltVacunaAntitetanica = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "En Progreso", Value = "1" },
                        new ListItem { Text = "Completa", Value = "2"},
                       }, "Value", "Text");
                ViewBag.ltVacunaAntitetanica = ltVacunaAntitetanica;

                var ltExamenPsifisiologico = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "Habilitada", Value = "1" },
                        new ListItem { Text = "No Habilitada", Value = "2"},
                       }, "Value", "Text");
                ViewBag.ltExamenPsifisiologico = ltExamenPsifisiologico;

                var ltSiNoInstruccionPrevia = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Si", Value = "1", Selected = setTASEstudiante.ltTASEstudiante.First().InstruccionPrevia },
                        new ListItem { Text = "No", Value = "2", Selected = !setTASEstudiante.ltTASEstudiante.First().InstruccionPrevia },
                   }, "Value", "Text" );
                ViewBag.ltSiNoInstruccionPrevia = ltSiNoInstruccionPrevia;

                var valueExperienciaPrevia = setTASEstudiante.ltTASEstudiante.First().ExperienciaPrevia ? "1" : "2";
                var ltSiNoExperienciaPrevia = new SelectList(
                       new List<ListItem>
                       {
                        new ListItem { Text = "Si", Value = "1", Selected = setTASEstudiante.ltTASEstudiante.First().ExperienciaPrevia },
                        new ListItem { Text = "No", Value = "2", Selected = !setTASEstudiante.ltTASEstudiante.First().ExperienciaPrevia},
                       }, "Value", "Text");
                ViewBag.ltSiNoExperienciaPrevia = ltSiNoExperienciaPrevia;

                ViewBag.MensajeError = MensajeError;

                return View(setTASEstudiante);
            }
        }

        [HttpGet]
        public ActionResult TASEstudianteAsignacionMaterias(string MensajeError = "")
        {
            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            List<SelectListItem> Estudiante = new List<SelectListItem>() {
                new SelectListItem() { Value="0", Text="Seleccione un Estudiante" },
           };

            ViewBag.Estudiante = Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View();
        }

        [HttpPost]
        public ActionResult TASEstudianteAsignacionMaterias(string Estudiante, string MensajeError = "")
        {
            //long NumError = 0;
            //string MensajeError = string.Empty;

            if (Estudiante != null && Estudiante != "")
            {
                return RedirectToAction("TASEstudianteAsignacionMateriasEstudiante", new { Estudiante = Estudiante });
            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("TASEstudianteAsignacionMaterias", new { MensajeError = MensajeError });
            }

        }

        [HttpGet]
        public ActionResult TASEstudianteAsignacionMateriasEstudiante(string Estudiante, string MensajeError = "")
        {
            TASEstudianteMateriasAsignacionInterfaceClient TASEstudianteMateriasAsignacion = new TASEstudianteMateriasAsignacionInterfaceClient();
            List<TASEstudianteMateriasAsignacionEntity> ltTASEstudianteMateriasAsignacion = new List<TASEstudianteMateriasAsignacionEntity>();

            if (Estudiante != null && Estudiante != "")
            {
                ltTASEstudianteMateriasAsignacion = TASEstudianteMateriasAsignacion.WebTASEstudianteMateriasAsignacionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltTASEstudianteMateriasAsignacion = ltTASEstudianteMateriasAsignacion.Where(x => x.EstudianteCI == Estudiante).ToList();
            }

            //TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            //var ltTASMateria = TASMateria.WebTASMateriaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //ViewBag.ltTASMateria = ltTASMateria.Where(x => ltTASEstudianteMateriasAsignacion.All(y => y.Materia != x.Materia)).ToList();

            ViewBag.EstudianteCI = Estudiante;
            ViewBag.MensajeError = MensajeError;

            return View(ltTASEstudianteMateriasAsignacion);
        }

        [HttpGet]
        public ActionResult TASEstudianteAsignacionMateriasEstudianteNuevo(string EstudianteCI)
        {
            long NumError = 0;
            if (EstudianteCI == null || EstudianteCI == "")
            {
                string MensajeError = "Existio un error al tratar de agregar un nuevo registro.";
                return RedirectToAction("TASEstudianteAsignacionMaterias", new { MensajeError = MensajeError });
            }

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            var ltTASMateria = TASMateria.WebTASMateriaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia);

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            var ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo);
            ViewBag.EstudianteCI = EstudianteCI;

            return PartialView();
        }

        [HttpPost]
        public ActionResult TASEstudianteAsignacionMateriasEstudianteNuevo(string EstudianteCI, string Materia, string Grupo)
        {
            long NumError = 0;
            string MensajeError = "";
            if (EstudianteCI == null || EstudianteCI == "" || Materia == null || Materia == "" || Grupo == null || Grupo == "")
            {
                MensajeError = "Debe seleccionar Materia y grupo.";
                NumError = 1;
            }

            if (NumError == 0)
            {
                TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
                var setTASEstudiante = TASEstudiante.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();

                TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
                var setTASMateria = TASMateria.WebSeleccionar((string)Session["SesionSubCompania"], Materia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etTASMateria = setTASMateria.ltTASMateria.First();

                TASEstudianteMateriasAsignacionInterfaceClient TASEstudianteMateriasAsignacion = new TASEstudianteMateriasAsignacionInterfaceClient();
                var setTASEstudianteMateriasAsignacion = TASEstudianteMateriasAsignacion.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                var etTASEstudianteMateriasAsignacion = setTASEstudianteMateriasAsignacion.ltTASEstudianteMateriasAsignacion.First();
                etTASEstudianteMateriasAsignacion.SubCompania = (string)Session["SesionSubCompania"];
                etTASEstudianteMateriasAsignacion.EstudianteCI = EstudianteCI;
                etTASEstudianteMateriasAsignacion.Programa = etTASEstudiante.Programa;
                etTASEstudianteMateriasAsignacion.Grupo = Grupo;
                etTASEstudianteMateriasAsignacion.Materia = Materia;
                etTASEstudianteMateriasAsignacion.ModuloMateria = etTASMateria.Modulo;
                etTASEstudianteMateriasAsignacion.UEstado = TASEstudianteMateriasAsignacionService.Estado.Added;

                NumError = TASEstudianteMateriasAsignacion.WebGuardar(setTASEstudianteMateriasAsignacion, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                MensajeError = "No se registro correctamente, verifique que los datos esten correctos y llenos.";
            }

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("TASEstudianteAsignacionMateriasEstudiante", "TASEstudiante", new { Estudiante = EstudianteCI } ) });
                //return RedirectToAction("TASRegistroNotasEditar", new { SubCompania = etTASRegistroNotasEstudianteForm.SubCompania, Programa = etTASRegistroNotasEstudianteForm.Programa, Grupo = etTASRegistroNotasEstudianteForm.Grupo, Materia = etTASRegistroNotasEstudianteForm.Materia, ModuloMateria = etTASRegistroNotasEstudianteForm.ModuloMateria, Gestion = etTASRegistroNotasEstudianteForm.Gestion });
            }
            else
            {
                TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
                var ltTASMateria = TASMateria.WebTASMateriaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia);

                TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
                var ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo);
                ViewBag.EstudianteCI = EstudianteCI;
                ViewBag.MensajeError = MensajeError;
                return PartialView();
            }
        }

        public JsonResult ObtenerEstudiantesGrupo(string Grupo)
        {
            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarXGrupo((string)Session["SesionSubCompania"], Grupo, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo).ToList();

            return Json(new { success = true, ltTASEstudiante = ltTASEstudiante }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TASEstudiantePDF(string EstudianteCI)
        {          
            long NumError = 0;
            string MensajeError = string.Empty;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;

            MapPath = "~/Reports/TASEstudiante/TASEstudianteReporte.rpt";
            FileDownloadName = "TASEstudianteCalificaciones_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";

            if (EstudianteCI != "")
            {
                try
                {
                    TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
                    var setTASEstudiante = TASEstudiante.WebSeleccionar((string)Session["SesionSubCompania"], EstudianteCI, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    var etTASEstudiante = setTASEstudiante.ltTASEstudiante.First();

                    ReportDocument report = new ReportDocument();
                    report.Load(Server.MapPath(MapPath));
                    report.SetDataSource(etTASEstudiante);
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
                        return RedirectToAction("TASEstudianteEditar", new { SubCompania = (string)Session["SesionSubCompania"], EstudianteCI = EstudianteCI, MensajeError = "Error al generar el reporte." });
                    }



                }
                catch (Exception e)
                {
                    return RedirectToAction("TASEstudianteEditar", new { SubCompania = (string)Session["SesionSubCompania"], EstudianteCI = EstudianteCI, MensajeError = e.Message });
                }

            }
            else
            {
                MensajeError = "Debe seleccionar un estudiante.";
                return RedirectToAction("TASEstudianteLista", new { SubCompania = (string)Session["SesionSubCompania"], EstudianteCI = EstudianteCI, MensajeError = MensajeError });
            }

        }
    }
}