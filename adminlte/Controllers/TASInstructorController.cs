using adminlte.TASGrupoService;
using adminlte.TASInstructorService;
using adminlte.TASMateriaService;
using adminlte.TASProgramacionHorarioInstructorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace adminlte.Controllers
{
    public class TASInstructorController : BaseController
    {
        // GET: TASInstructor
        public ActionResult TASInstructorLista()
        {
            TASInstructorInterfaceClient TASInstructor = new TASInstructorInterfaceClient();
            var ltTASInstructor = TASInstructor.WebTASInstructorSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASInstructor);
        }

        [HttpGet]
        public ActionResult TASInstructorNuevo(string MensajeError = "")
        {
            TASInstructorInterfaceClient TASInstructor = new TASInstructorInterfaceClient();
            TASInstructorSet setTASInstructor = TASInstructor.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

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

            ViewBag.MensajeError = MensajeError;

            return View(setTASInstructor);
        }

        [HttpPost]
        public ActionResult TASInstructorNuevo(TASInstructorSet setTASInstructor,
            string Sexo, string VacunaAntitetanica)
        {
            long NumError = 0;
            string InstructorNroLicencia = string.Empty;
            string MensajeError = string.Empty;

            TASInstructorInterfaceClient TASInstructor = new TASInstructorInterfaceClient();

            if (setTASInstructor.ltTASInstructor != null)
            {
                TASInstructorEntity etTASInstructor = setTASInstructor.ltTASInstructor.First();
                if (etTASInstructor.InstructorNroLicencia == null || etTASInstructor.InstructorNroLicencia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar un NroLicencia. ";
                }

                //if (VacunaAntitetanica == null || VacunaAntitetanica == "")
                //{
                //    NumError = 1;
                //    MensajeError = "Debe llenar el campo Vacuna Antitetanica.";
                //}

                if (NumError == 0)
                {
                    TASInstructorSet setTASInstructorNuevo = TASInstructor.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASInstructorEntity etTASInstructorNuevo = setTASInstructorNuevo.ltTASInstructor.First();
                    etTASInstructorNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASInstructorNuevo.InstructorNroLicencia = etTASInstructor.InstructorNroLicencia == null ? "" : etTASInstructor.InstructorNroLicencia;
                    etTASInstructorNuevo.Fecha = DateTime.Now;
                    //CodigoTAS Same as EstudianteCI
                    etTASInstructorNuevo.CodigoTAS = etTASInstructor.InstructorNroLicencia == null ? "" : etTASInstructor.InstructorNroLicencia;
                    etTASInstructorNuevo.Nombre1 = etTASInstructor.Nombre1 == null ? "" : etTASInstructor.Nombre1;
                    etTASInstructorNuevo.Nombre2 = etTASInstructor.Nombre2 == null ? "" : etTASInstructor.Nombre2;
                    etTASInstructorNuevo.Apellido1 = etTASInstructor.Apellido1 == null ? "" : etTASInstructor.Apellido1;
                    etTASInstructorNuevo.Apellido2 = etTASInstructor.Apellido2 == null ? "" : etTASInstructor.Apellido2;
                    etTASInstructorNuevo.LugarNacimiento = etTASInstructor.LugarNacimiento == null ? "" : etTASInstructor.LugarNacimiento;
                    etTASInstructorNuevo.Sexo = Sexo == null ? "" : Sexo;
                    etTASInstructorNuevo.FechaNacimiento = etTASInstructor.FechaNacimiento;
                    etTASInstructorNuevo.Nacionalidad = etTASInstructor.Nacionalidad == null ? "" : etTASInstructor.Nacionalidad;
                    etTASInstructorNuevo.EstadoCivil = etTASInstructor.EstadoCivil == null ? "" : etTASInstructor.EstadoCivil;
                    etTASInstructorNuevo.Domicilio = etTASInstructor.Domicilio == null ? "" : etTASInstructor.Domicilio;
                    etTASInstructorNuevo.Telefono = etTASInstructor.Telefono == null ? "" : etTASInstructor.Telefono;
                    etTASInstructorNuevo.Correo = etTASInstructor.Correo == null ? "" : etTASInstructor.Correo;
                    etTASInstructorNuevo.Formacion = etTASInstructor.Formacion == null ? "" : etTASInstructor.Formacion;
                    etTASInstructorNuevo.Cursos = etTASInstructor.Cursos == null ? "" : etTASInstructor.Cursos;
                    etTASInstructorNuevo.ExperienciaLaboral = etTASInstructor.ExperienciaLaboral == null ? "" : etTASInstructor.ExperienciaLaboral;
                    etTASInstructorNuevo.ExperienciaInstruccion = etTASInstructor.ExperienciaInstruccion == null ? "" : etTASInstructor.ExperienciaInstruccion;
                    
                    etTASInstructorNuevo.VacunaAntitetanica = VacunaAntitetanica == null ? "" : VacunaAntitetanica;
                    etTASInstructorNuevo.Observaciones = etTASInstructor.Observaciones == null ? "" : etTASInstructor.Observaciones;

                    etTASInstructorNuevo.UEstado = TASInstructorService.Estado.Added;
                    InstructorNroLicencia = etTASInstructor.InstructorNroLicencia;
                    NumError = TASInstructor.WebGuardar(setTASInstructorNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASInstructorEditar", new { SubCompania = (string)Session["SesionSubCompania"], InstructorNroLicencia = InstructorNroLicencia });
            }
            else
            {
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
                ViewBag.MensajeError = MensajeError;

                return View(setTASInstructor);
            }

        }

        [HttpGet]
        public ActionResult TASInstructorEditar(string SubCompania, string InstructorNroLicencia, string MensajeError = "")
        {
            TASInstructorInterfaceClient TASInstructor = new TASInstructorInterfaceClient();
            TASInstructorSet setTASInstructor = new TASInstructorSet();

            setTASInstructor = TASInstructor.WebSeleccionar(SubCompania, InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltSexo = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Femenino", Value = "F" , Selected = setTASInstructor.ltTASInstructor.First().Sexo == "F" ? true : false },
                        new ListItem { Text = "Masculino", Value = "M", Selected = setTASInstructor.ltTASInstructor.First().Sexo == "M" ? true : false},
                   }, "Value", "Text", setTASInstructor.ltTASInstructor.First().Sexo); ;
            //ltSexo.Where(x => x.Value == setTASInstructor.ltTASInstructor.First().Sexo).First().Selected = true;
            //ViewBag.ltSexo = ltSexo;

            var ltVacunaAntitetanica = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "En Progreso", Value = "1", Selected = setTASInstructor.ltTASInstructor.First().VacunaAntitetanica == "1" ? true : false},
                        new ListItem { Text = "Completa", Value = "2", Selected = setTASInstructor.ltTASInstructor.First().VacunaAntitetanica == "2" ? true : false},
                   }, "Value", "Text", setTASInstructor.ltTASInstructor.First().VacunaAntitetanica);

            //var etVacunaAntitetanica = ltVacunaAntitetanica.Where(x => x.Value == valueVacunaAntitetanica).FirstOrDefault();
            //etVacunaAntitetanica.Selected = true;
            ViewBag.ltVacunaAntitetanica = ltVacunaAntitetanica;
            ViewBag.MensajeError = MensajeError;

            return View(setTASInstructor);
        }

        [HttpPost]
        public ActionResult TASInstructorEditar(TASInstructorSet setTASInstructor,
            string Sexo, string VacunaAntitetanica)
        {
            long NumError = 0;
            string InstructorNroLicencia = string.Empty;
            string MensajeError = string.Empty;

            TASInstructorInterfaceClient TASInstructor = new TASInstructorInterfaceClient();

            if (setTASInstructor.ltTASInstructor != null)
            {
                TASInstructorEntity etTASInstructor = setTASInstructor.ltTASInstructor.First();
                if (etTASInstructor.InstructorNroLicencia == null || etTASInstructor.InstructorNroLicencia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe existir un NroLicencia. ";
                }

                //if (etTASInstructor.Programa == null || etTASInstructor.Programa == "")
                //{
                //    NumError = 1;
                //    MensajeError += "Debe seleccionar un Programa. ";
                //}

                if (NumError == 0)
                {
                    TASInstructorSet setTASInstructorEdit = TASInstructor.WebSeleccionar((string)Session["SesionSubCompania"], etTASInstructor.InstructorNroLicencia,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASInstructorEntity etTASInstructorEdit = setTASInstructorEdit.ltTASInstructor.First();
                    etTASInstructorEdit.SubCompania = (string)Session["SesionSubCompania"];
                    etTASInstructorEdit.Fecha = DateTime.Now;
                    //CodigoTAS Same as EstudianteCI
                    etTASInstructorEdit.CodigoTAS = etTASInstructor.InstructorNroLicencia == null ? "" : etTASInstructor.InstructorNroLicencia;
                    etTASInstructorEdit.Nombre1 = etTASInstructor.Nombre1 == null ? "" : etTASInstructor.Nombre1;
                    etTASInstructorEdit.Nombre2 = etTASInstructor.Nombre2 == null ? "" : etTASInstructor.Nombre2;
                    etTASInstructorEdit.Apellido1 = etTASInstructor.Apellido1 == null ? "" : etTASInstructor.Apellido1;
                    etTASInstructorEdit.Apellido2 = etTASInstructor.Apellido2 == null ? "" : etTASInstructor.Apellido2;
                    etTASInstructorEdit.LugarNacimiento = etTASInstructor.LugarNacimiento == null ? "" : etTASInstructor.LugarNacimiento;
                    etTASInstructorEdit.Sexo = Sexo == null ? "" : Sexo;
                    etTASInstructorEdit.FechaNacimiento = etTASInstructor.FechaNacimiento;
                    etTASInstructorEdit.Nacionalidad = etTASInstructor.Nacionalidad == null ? "" : etTASInstructor.Nacionalidad;
                    etTASInstructorEdit.EstadoCivil = etTASInstructor.EstadoCivil == null ? "" : etTASInstructor.EstadoCivil;
                    etTASInstructorEdit.Domicilio = etTASInstructor.Domicilio == null ? "" : etTASInstructor.Domicilio;
                    etTASInstructorEdit.Telefono = etTASInstructor.Telefono == null ? "" : etTASInstructor.Telefono;
                    etTASInstructorEdit.Correo = etTASInstructor.Correo == null ? "" : etTASInstructor.Correo;
                    etTASInstructorEdit.Formacion = etTASInstructor.Formacion == null ? "" : etTASInstructor.Formacion;
                    etTASInstructorEdit.Cursos = etTASInstructor.Cursos == null ? "" : etTASInstructor.Cursos;
                    etTASInstructorEdit.ExperienciaLaboral = etTASInstructor.ExperienciaLaboral == null ? "" : etTASInstructor.ExperienciaLaboral;
                    etTASInstructorEdit.ExperienciaInstruccion = etTASInstructor.ExperienciaInstruccion == null ? "" : etTASInstructor.ExperienciaInstruccion;

                    etTASInstructorEdit.VacunaAntitetanica = VacunaAntitetanica == null ? "" : VacunaAntitetanica;
                    etTASInstructorEdit.Observaciones = etTASInstructor.Observaciones == null ? "" : etTASInstructor.Observaciones;

                    etTASInstructorEdit.UEstado = TASInstructorService.Estado.Modified;
                    InstructorNroLicencia = etTASInstructor.InstructorNroLicencia;
                    NumError = TASInstructor.WebGuardar(setTASInstructorEdit, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASInstructorLista");
            }
            else
            {
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
                ViewBag.MensajeError = MensajeError;

                return View(setTASInstructor);
            }
        }

        [HttpGet]
        public ActionResult TASProgramacionHorarioInstructorLista()
        {
            TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
            var ltTASProgramacionHorarioInstructor = TASProgramacionHorarioInstructor.WebTASProgramacionHorarioInstructorSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASProgramacionHorarioInstructor);
        }

        [HttpGet]
        public ActionResult TASProgramacionHorarioInstructorEditar(string SubCompania, string InstructorNroLicencia)
        {
            TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
            var setTASProgramacionHorarioInstructor = TASProgramacionHorarioInstructor.WebSeleccionar(SubCompania,InstructorNroLicencia,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(setTASProgramacionHorarioInstructor);
        }

        [HttpGet]
        public ActionResult TASProgramacionHorarioInstructorLineaNuevo(string SubCompania, string InstructorNroLicencia, string MensajeError = "")
        {
            TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLinea = new TASProgramacionHorarioInstructorLineaEntity();
            etTASProgramacionHorarioInstructorLinea.SubCompania = SubCompania;
            etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia = InstructorNroLicencia;
            etTASProgramacionHorarioInstructorLinea.Linea = DateTime.Now.Ticks;
            etTASProgramacionHorarioInstructorLinea.Materia = "";
            etTASProgramacionHorarioInstructorLinea.Grupo = "";
            etTASProgramacionHorarioInstructorLinea.FechaInicio = DateTime.Now;
            etTASProgramacionHorarioInstructorLinea.FechaFin = DateTime.Now;
            etTASProgramacionHorarioInstructorLinea.Aula = "";

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            List<TASMateriaEntity> ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia).ToList();

            return PartialView(etTASProgramacionHorarioInstructorLinea);
        }
        [HttpPost]
        public ActionResult TASProgramacionHorarioInstructorLineaNuevo(TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLinea, DateTime FechaDoc)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLineaOriginal = etTASProgramacionHorarioInstructorLinea;

            if (etTASProgramacionHorarioInstructorLinea.FechaInicio > etTASProgramacionHorarioInstructorLinea.FechaFin)
            {
                NumError = 1;
                MensajeError += " La hora inicial es mayor a la hora final.";
            }

            if (etTASProgramacionHorarioInstructorLinea.Materia == null || etTASProgramacionHorarioInstructorLinea.Materia == "")
            {
                NumError = 1;
                MensajeError += " Debe seleccionar una Materia.";
            }

            if (etTASProgramacionHorarioInstructorLinea.Grupo == null || etTASProgramacionHorarioInstructorLinea.Grupo == "")
            {
                NumError = 1;
                MensajeError += " Debe seleccionar un Grupo.";
            }

            if (NumError == 0 )
            {
                TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
                var setTASProgramacionHorarioInstructor = TASProgramacionHorarioInstructor.WebSeleccionar(etTASProgramacionHorarioInstructorLinea.SubCompania, etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLineaAdd = new TASProgramacionHorarioInstructorLineaEntity();
                etTASProgramacionHorarioInstructorLineaAdd.SubCompania = etTASProgramacionHorarioInstructorLinea.SubCompania;
                etTASProgramacionHorarioInstructorLineaAdd.InstructorNroLicencia = etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia;
                etTASProgramacionHorarioInstructorLineaAdd.Linea = DateTime.Now.Ticks;
                etTASProgramacionHorarioInstructorLineaAdd.Materia = etTASProgramacionHorarioInstructorLinea.Materia;
                etTASProgramacionHorarioInstructorLineaAdd.Grupo = etTASProgramacionHorarioInstructorLinea.Grupo;
                etTASProgramacionHorarioInstructorLineaAdd.FechaInicio = FechaDoc.Date + etTASProgramacionHorarioInstructorLinea.FechaInicio.TimeOfDay;
                etTASProgramacionHorarioInstructorLineaAdd.FechaFin = FechaDoc.Date + etTASProgramacionHorarioInstructorLinea.FechaFin.TimeOfDay;
                etTASProgramacionHorarioInstructorLineaAdd.Aula = etTASProgramacionHorarioInstructorLinea.Aula == null ? "" : etTASProgramacionHorarioInstructorLinea.Aula;
                etTASProgramacionHorarioInstructorLineaAdd.UEstado = TASProgramacionHorarioInstructorService.Estado.Added;

                setTASProgramacionHorarioInstructor.ltTASProgramacionHorarioInstructorLinea.Add(etTASProgramacionHorarioInstructorLineaAdd);

                NumError = TASProgramacionHorarioInstructor.WebGuardar(setTASProgramacionHorarioInstructor, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (NumError != 0)
                {
                    MensajeError += "Ocurrio un error al intentar guardar. Intente nuevamente.";
                }
            }


            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("TASProgramacionHorarioInstructorEditar", "TASInstructor", new { SubCompania = etTASProgramacionHorarioInstructorLinea.SubCompania, InstructorNroLicencia = etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia }) });
                //return PartialView("ASAConfiguracionSuccess", new { SubCompania = etASAConfiguracionLinea.SubCompania, Grupo = etASAConfiguracionLinea.Grupo });
            }
            else
            {
                TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
                List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

                TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
                List<TASMateriaEntity> ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia).ToList();

                ViewBag.MensajeError = MensajeError;
                return PartialView(etTASProgramacionHorarioInstructorLineaOriginal);
            }
        }

        [HttpGet]
        public ActionResult TASProgramacionHorarioInstructorLineaEditar(string SubCompania, string InstructorNroLicencia, long Linea, string MensajeError = "")
        {
            TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
            TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLinea = new TASProgramacionHorarioInstructorLineaEntity();

            TASProgramacionHorarioInstructorSet setTASProgramacionHorarioInstructor = TASProgramacionHorarioInstructor.WebSeleccionar(SubCompania, InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            etTASProgramacionHorarioInstructorLinea = setTASProgramacionHorarioInstructor.ltTASProgramacionHorarioInstructorLinea.Where(x => x.SubCompania == SubCompania && x.InstructorNroLicencia == InstructorNroLicencia && x.Linea == Linea).First();

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            List<TASMateriaEntity> ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia).ToList();
            ViewBag.MensajeError = MensajeError;

            return PartialView(etTASProgramacionHorarioInstructorLinea);
        }

        [HttpPost]
        public ActionResult TASProgramacionHorarioInstructorLineaEditar(TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLinea, DateTime FechaDoc)
        {
            long NumError = 0;
            string MensajeError = string.Empty;
            TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLineaOriginal = etTASProgramacionHorarioInstructorLinea;

            if (etTASProgramacionHorarioInstructorLinea.FechaInicio > etTASProgramacionHorarioInstructorLinea.FechaFin)
            {
                NumError = 1;
                MensajeError += " La hora inicial es mayor a la hora final.";
            }

            if (etTASProgramacionHorarioInstructorLinea.Materia == null || etTASProgramacionHorarioInstructorLinea.Materia == "")
            {
                NumError = 1;
                MensajeError += " Debe seleccionar una Materia.";
            }

            if (etTASProgramacionHorarioInstructorLinea.Grupo == null || etTASProgramacionHorarioInstructorLinea.Grupo == "")
            {
                NumError = 1;
                MensajeError += " Debe seleccionar un Grupo.";
            }

            if (NumError == 0)
            {
                TASProgramacionHorarioInstructorInterfaceClient TASProgramacionHorarioInstructor = new TASProgramacionHorarioInstructorInterfaceClient();
                var setTASProgramacionHorarioInstructor = TASProgramacionHorarioInstructor.WebSeleccionar(etTASProgramacionHorarioInstructorLinea.SubCompania, etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                TASProgramacionHorarioInstructorLineaEntity etTASProgramacionHorarioInstructorLineaAdd = setTASProgramacionHorarioInstructor.ltTASProgramacionHorarioInstructorLinea.Where(x => x.SubCompania == etTASProgramacionHorarioInstructorLinea.SubCompania && x.InstructorNroLicencia == etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia && x.Linea == etTASProgramacionHorarioInstructorLinea.Linea).First();
                etTASProgramacionHorarioInstructorLineaAdd.SubCompania = etTASProgramacionHorarioInstructorLinea.SubCompania;
                etTASProgramacionHorarioInstructorLineaAdd.InstructorNroLicencia = etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia;
                etTASProgramacionHorarioInstructorLineaAdd.Linea = etTASProgramacionHorarioInstructorLinea.Linea;
                etTASProgramacionHorarioInstructorLineaAdd.Materia = etTASProgramacionHorarioInstructorLinea.Materia;
                etTASProgramacionHorarioInstructorLineaAdd.Grupo = etTASProgramacionHorarioInstructorLinea.Grupo;
                etTASProgramacionHorarioInstructorLineaAdd.FechaInicio = FechaDoc.Date + etTASProgramacionHorarioInstructorLinea.FechaInicio.TimeOfDay;
                etTASProgramacionHorarioInstructorLineaAdd.FechaFin = FechaDoc.Date + etTASProgramacionHorarioInstructorLinea.FechaFin.TimeOfDay;
                etTASProgramacionHorarioInstructorLineaAdd.Aula = etTASProgramacionHorarioInstructorLinea.Aula == null ? "" : etTASProgramacionHorarioInstructorLinea.Aula;
                etTASProgramacionHorarioInstructorLineaAdd.UEstado = TASProgramacionHorarioInstructorService.Estado.Modified;

                //setTASProgramacionHorarioInstructor.ltTASProgramacionHorarioInstructorLinea.Add(etTASProgramacionHorarioInstructorLineaAdd);

                NumError = TASProgramacionHorarioInstructor.WebGuardar(setTASProgramacionHorarioInstructor, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                if (NumError != 0)
                {
                    MensajeError += "Ocurrio un error al intentar guardar. Intente nuevamente.";
                }
            }


            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("TASProgramacionHorarioInstructorEditar", "TASInstructor", new { SubCompania = etTASProgramacionHorarioInstructorLinea.SubCompania, InstructorNroLicencia = etTASProgramacionHorarioInstructorLinea.InstructorNroLicencia }) });
                //return PartialView("ASAConfiguracionSuccess", new { SubCompania = etASAConfiguracionLinea.SubCompania, Grupo = etASAConfiguracionLinea.Grupo });
            }
            else
            {
                TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
                List<TASGrupoEntity> ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASGrupo = ltTASGrupo.Select(s => s.Grupo).ToList();

                TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
                List<TASMateriaEntity> ltTASMateria = TASMateria.WebTASMateriaSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ViewBag.ltTASMateria = ltTASMateria.Select(s => s.Materia).ToList();

                ViewBag.MensajeError = MensajeError;
                return PartialView(etTASProgramacionHorarioInstructorLineaOriginal);
            }
        }

    }
}