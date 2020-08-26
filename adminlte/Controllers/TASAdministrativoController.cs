using adminlte.TASAdministrativoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace adminlte.Controllers
{
    public class TASAdministrativoController : BaseController
    {
        // GET: TASAdministrativo
        public ActionResult TASAdministrativoLista()
        {
            TASAdministrativoInterfaceClient TASAdministrativo = new TASAdministrativoInterfaceClient();
            var ltTASAdministrativo = TASAdministrativo.WebTASAdministrativoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(ltTASAdministrativo);
        }

        [HttpGet]
        public ActionResult TASAdministrativoNuevo(string MensajeError = "")
        {
            TASAdministrativoInterfaceClient TASAdministrativo = new TASAdministrativoInterfaceClient();
            TASAdministrativoSet setTASAdministrativo = TASAdministrativo.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            //List<TASAdministrativoEntity> ltTASAdministrativo = TASAdministrativo.WebTASAdministrativoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);                      

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

            return View(setTASAdministrativo);
        }

        [HttpPost]
        public ActionResult TASAdministrativoNuevo(TASAdministrativoSet setTASAdministrativo,
            string Sexo, string VacunaAntitetanica)
        {
            long NumError = 0;
            string AdminNroLicencia = string.Empty;
            string MensajeError = string.Empty;

            TASAdministrativoInterfaceClient TASAdministrativo = new TASAdministrativoInterfaceClient();

            if (setTASAdministrativo.ltTASAdministrativo != null)
            {
                TASAdministrativoEntity etTASAdministrativo = setTASAdministrativo.ltTASAdministrativo.First();                

                if (etTASAdministrativo.AdminNroLicencia == null || etTASAdministrativo.AdminNroLicencia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar el CI del Admin. ";
                }                

                if (NumError == 0)
                {
                    TASAdministrativoSet setTASAdministrativoNuevo = TASAdministrativo.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    TASAdministrativoEntity etTASAdministrativoNuevo = setTASAdministrativoNuevo.ltTASAdministrativo.First();
                    etTASAdministrativoNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    etTASAdministrativoNuevo.AdminNroLicencia = etTASAdministrativo.AdminNroLicencia == null ? "" : etTASAdministrativo.AdminNroLicencia;
                    //CodigoTAS Same as EstudianteCI
                    etTASAdministrativoNuevo.Fecha = DateTime.Now;
                    etTASAdministrativoNuevo.Nombre = etTASAdministrativo.Nombre == null ? "" : etTASAdministrativo.Nombre;
                    etTASAdministrativoNuevo.LugarNacimiento = etTASAdministrativo.LugarNacimiento == null ? "" : etTASAdministrativo.LugarNacimiento;
                    etTASAdministrativoNuevo.Sexo = Sexo == null ? "" : Sexo;
                    etTASAdministrativoNuevo.FechaNacimiento = etTASAdministrativo.FechaNacimiento;
                    etTASAdministrativoNuevo.Nacionalidad = etTASAdministrativo.Nacionalidad == null ? "" : etTASAdministrativo.Nacionalidad;
                    etTASAdministrativoNuevo.EstadoCivil = etTASAdministrativo.EstadoCivil == null ? "" : etTASAdministrativo.EstadoCivil;
                    etTASAdministrativoNuevo.Domicilio = etTASAdministrativo.Domicilio == null ? "" : etTASAdministrativo.Domicilio;
                    etTASAdministrativoNuevo.Telefono = etTASAdministrativo.Telefono == null ? "" : etTASAdministrativo.Telefono;
                    etTASAdministrativoNuevo.Correo = etTASAdministrativo.Correo == null ? "" : etTASAdministrativo.Correo;
                    etTASAdministrativoNuevo.Formacion = etTASAdministrativo.Formacion == null ? "" : etTASAdministrativo.Formacion;
                    etTASAdministrativoNuevo.Cursos = etTASAdministrativo.Cursos == null ? "" : etTASAdministrativo.Cursos;
                    etTASAdministrativoNuevo.ExperienciaLaboral = etTASAdministrativo.ExperienciaLaboral == null ? "" : etTASAdministrativo.ExperienciaLaboral;
                    etTASAdministrativoNuevo.ExperienciaInstruccion = etTASAdministrativo.ExperienciaInstruccion == null ? "" : etTASAdministrativo.ExperienciaInstruccion;

                    etTASAdministrativoNuevo.VacunaAntitetanica = VacunaAntitetanica == null ? "" : VacunaAntitetanica;                    
                    etTASAdministrativoNuevo.Observaciones = etTASAdministrativo.Observaciones == null ? "" : etTASAdministrativo.Observaciones;

                    etTASAdministrativoNuevo.UEstado = TASAdministrativoService.Estado.Added;
                    AdminNroLicencia = etTASAdministrativo.AdminNroLicencia;
                    NumError = TASAdministrativo.WebGuardar(setTASAdministrativoNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASAdministrativoEditar", new { SubCompania = (string)Session["SesionSubCompania"], AdminNroLicencia = AdminNroLicencia });
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

                return View(setTASAdministrativo);
            }

        }

        [HttpGet]
        public ActionResult TASAdministrativoEditar(string SubCompania, string AdminNroLicencia, string MensajeError = "")
        {
            TASAdministrativoInterfaceClient TASAdministrativo = new TASAdministrativoInterfaceClient();
            TASAdministrativoSet setTASAdministrativo = new TASAdministrativoSet();
            setTASAdministrativo = TASAdministrativo.WebSeleccionar(SubCompania, AdminNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltSexo = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "Femenino", Value = "F" , Selected = setTASAdministrativo.ltTASAdministrativo.First().Sexo == "F" ? true : false },
                        new ListItem { Text = "Masculino", Value = "M", Selected = setTASAdministrativo.ltTASAdministrativo.First().Sexo == "M" ? true : false},
                   }, "Value", "Text", setTASAdministrativo.ltTASAdministrativo.First().Sexo); ;
            //ltSexo.Where(x => x.Value == setTASAdministrativo.ltTASAdministrativo.First().Sexo).First().Selected = true;
            //ViewBag.ltSexo = ltSexo;

            //var valueVacunaAntitetanica = setTASAdministrativo.ltTASAdministrativo.First().VacunaAntitetanica ? "2" : "1";
            var ltVacunaAntitetanica = new SelectList(
                   new List<ListItem>
                   {
                        new ListItem { Text = "En Progreso", Value = "1", Selected = setTASAdministrativo.ltTASAdministrativo.First().VacunaAntitetanica == "1" ? true : false},
                        new ListItem { Text = "Completa", Value = "2", Selected = setTASAdministrativo.ltTASAdministrativo.First().VacunaAntitetanica == "2" ? true : false},
                   }, "Value", "Text", setTASAdministrativo.ltTASAdministrativo.First().VacunaAntitetanica);

            ViewBag.ltVacunaAntitetanica = ltVacunaAntitetanica;
            ViewBag.MensajeError = MensajeError;

            return View(setTASAdministrativo);
        }

        [HttpPost]
        public ActionResult TASAdministrativoEditar(TASAdministrativoSet setTASAdministrativo,
            string Sexo, string VacunaAntitetanica)
        {
            long NumError = 0;
            string AdminNroLicencia = string.Empty;
            string MensajeError = string.Empty;

            TASAdministrativoInterfaceClient TASAdministrativo = new TASAdministrativoInterfaceClient();

            if (setTASAdministrativo.ltTASAdministrativo != null)
            {
                TASAdministrativoEntity etTASAdministrativo = setTASAdministrativo.ltTASAdministrativo.First();
                
                if (etTASAdministrativo.AdminNroLicencia == null || etTASAdministrativo.AdminNroLicencia == "")
                {
                    NumError = 1;
                    MensajeError += "Debe ingresar el AdminNroLicencia. ";
                }
                
                if (NumError == 0)
                {
                    TASAdministrativoSet setTASAdministrativoNuevo = TASAdministrativo.WebSeleccionar((string)Session["SesionSubCompania"], etTASAdministrativo.AdminNroLicencia ,(string)Session["Sesion"],(string)Session["SesionSubCompania"]);
                    TASAdministrativoEntity etTASAdministrativoNuevo = setTASAdministrativoNuevo.ltTASAdministrativo.First();
                    etTASAdministrativoNuevo.SubCompania = (string)Session["SesionSubCompania"];
                    //CodigoTAS Same as EstudianteCI
                    etTASAdministrativoNuevo.Fecha = DateTime.Now;
                    etTASAdministrativoNuevo.Nombre = etTASAdministrativo.Nombre == null ? "" : etTASAdministrativo.Nombre;
                    etTASAdministrativoNuevo.LugarNacimiento = etTASAdministrativo.LugarNacimiento == null ? "" : etTASAdministrativo.LugarNacimiento;
                    etTASAdministrativoNuevo.Sexo = Sexo == null ? "" : Sexo;
                    etTASAdministrativoNuevo.FechaNacimiento = etTASAdministrativo.FechaNacimiento;
                    etTASAdministrativoNuevo.Nacionalidad = etTASAdministrativo.Nacionalidad == null ? "" : etTASAdministrativo.Nacionalidad;
                    etTASAdministrativoNuevo.EstadoCivil = etTASAdministrativo.EstadoCivil == null ? "" : etTASAdministrativo.EstadoCivil;
                    etTASAdministrativoNuevo.Domicilio = etTASAdministrativo.Domicilio == null ? "" : etTASAdministrativo.Domicilio;
                    etTASAdministrativoNuevo.Telefono = etTASAdministrativo.Telefono == null ? "" : etTASAdministrativo.Telefono;
                    etTASAdministrativoNuevo.Correo = etTASAdministrativo.Correo == null ? "" : etTASAdministrativo.Correo;
                    etTASAdministrativoNuevo.Formacion = etTASAdministrativo.Formacion == null ? "" : etTASAdministrativo.Formacion;
                    etTASAdministrativoNuevo.Cursos = etTASAdministrativo.Cursos == null ? "" : etTASAdministrativo.Cursos;
                    etTASAdministrativoNuevo.ExperienciaLaboral = etTASAdministrativo.ExperienciaLaboral == null ? "" : etTASAdministrativo.ExperienciaLaboral;
                    etTASAdministrativoNuevo.ExperienciaInstruccion = etTASAdministrativo.ExperienciaInstruccion == null ? "" : etTASAdministrativo.ExperienciaInstruccion;

                    etTASAdministrativoNuevo.VacunaAntitetanica = VacunaAntitetanica == null ? "" : VacunaAntitetanica;
                    etTASAdministrativoNuevo.Observaciones = etTASAdministrativo.Observaciones == null ? "" : etTASAdministrativo.Observaciones;

                    etTASAdministrativoNuevo.UEstado = TASAdministrativoService.Estado.Modified;
                    AdminNroLicencia = etTASAdministrativo.AdminNroLicencia;
                    NumError = TASAdministrativo.WebGuardar(setTASAdministrativoNuevo, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                }
            }
            else
            {
                NumError = 1;
                MensajeError = "Ocurrio un error al intentar guardar. Intente nuevamente.";
            }

            if (NumError == 0)
            {
                return RedirectToAction("TASAdministrativoLista");
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

                return View(setTASAdministrativo);
            }
        }
    }
}