using adminlte.TASEstudianteService;
using adminlte.TASGestionService;
using adminlte.TASGrupoService;
using adminlte.TASMateriaService;
using adminlte.TASModuloService;
using adminlte.TASProgramaService;
using adminlte.TASRegistroNotasService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASRegistroNotasController : BaseController
    {
        // GET: TASRegistroNota
        public ActionResult TASRegistroNotasLista()
        {
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            List<TASRegistroNotasEntity> ltTASRegistroNotas = TASRegistroNotas.WebTASRegistroNotasSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASRegistroNotas = ltTASRegistroNotas.Where(x => x.Estado == "Abierto" && x.InstructorLic == (string)Session["Usuario"]).ToList();

            return View(ltTASRegistroNotas);
        }
        public ActionResult TASRegistroNotasNuevo()
        {
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASProgramaInterfaceClient TASPrograma = new TASProgramaInterfaceClient();
            ViewBag.ltTASPrograma = TASPrograma.WebTASProgramaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASGrupoInterfaceClient TASGrupo = new TASGrupoInterfaceClient();
            ViewBag.ltTASGrupo = TASGrupo.WebTASGrupoSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASMateriaInterfaceClient TASMateria = new TASMateriaInterfaceClient();
            ViewBag.ltTASMateria = TASMateria.WebTASMateriaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASModuloInterfaceClient TASModulo = new TASModuloInterfaceClient();
            ViewBag.ltTASModulo = TASModulo.WebTASModuloSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASGestionInterfaceClient TASGestion = new TASGestionInterfaceClient();
            ViewBag.ltTASGestion = TASGestion.WebTASGestionSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            ViewBag.ltCantidadProgresos = Enumerable.Range(1, 40).ToList();

            return View(setTASRegistroNotas);
        }
        [HttpPost]
        public ActionResult TASRegistroNotasNuevo(TASRegistroNotasSet setTASRegistroNotas)
        {
            long NumError = 0;
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasEntity etTASRegistroNotas = new TASRegistroNotasEntity();
            if (setTASRegistroNotas.ltTASRegistroNotas != null)
            {
                TASRegistroNotasEntity etTASRegistroNotasForm = setTASRegistroNotas.ltTASRegistroNotas.First();
                if (etTASRegistroNotasForm.Programa != "" && etTASRegistroNotasForm.Grupo != "" && etTASRegistroNotasForm.Materia != "" && etTASRegistroNotasForm.ModuloMateria != "" && etTASRegistroNotasForm.Gestion != "")
                {
                    TASRegistroNotasSet setTASRegistroNotasNuevo = TASRegistroNotas.WebNuevo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    etTASRegistroNotas = setTASRegistroNotasNuevo.ltTASRegistroNotas.First();
                    etTASRegistroNotas.SubCompania = (string)Session["SesionSubCompania"];
                    etTASRegistroNotas.Programa = etTASRegistroNotasForm.Programa;
                    etTASRegistroNotas.Grupo = etTASRegistroNotasForm.Grupo;
                    etTASRegistroNotas.Materia = etTASRegistroNotasForm.Materia;
                    etTASRegistroNotas.ModuloMateria = etTASRegistroNotasForm.ModuloMateria;
                    etTASRegistroNotas.Gestion = etTASRegistroNotasForm.Gestion;
                    etTASRegistroNotas.InstructorLic = (string)Session["Usuario"];
                    etTASRegistroNotas.Estado = "Abierto";
                    //configuracion
                    etTASRegistroNotas.PorcentajeProgreso1P = 30;
                    etTASRegistroNotas.PorcentajeDominio1D = 70;
                    etTASRegistroNotas.NumeroProgresos = etTASRegistroNotasForm.NumeroProgresos;
                    etTASRegistroNotas.NumeroDominios = 1;
                    etTASRegistroNotas.NombreReferenciaProgreso = etTASRegistroNotasForm.NombreReferenciaProgreso.Substring(11);
                    etTASRegistroNotas.NombreReferenciaDominio = "Examen de Dominio";
                    etTASRegistroNotas.PorcentajeIndividualDominio = "70";
                    etTASRegistroNotas.PorcentajeIndividualProgreso = etTASRegistroNotasForm.PorcentajeIndividualProgreso.Substring(11);

                    etTASRegistroNotas.UEstado = TASRegistroNotasService.Estado.Added;

                    NumError = TASRegistroNotas.WebGuardar(setTASRegistroNotasNuevo, true, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
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
                return RedirectToAction("TASRegistroNotasEditar", new { SubCompania = etTASRegistroNotas.SubCompania, Programa = etTASRegistroNotas.Programa, Grupo = etTASRegistroNotas.Grupo, Materia = etTASRegistroNotas.Materia, ModuloMateria = etTASRegistroNotas.ModuloMateria, Gestion = etTASRegistroNotas.Gestion});
            }
            else
            {
                return RedirectToAction("TASRegistroNotasLista");
            }
        }

        public ActionResult TASRegistroNotasEditar(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion)
        {
            //TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            //TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar(SubCompania,Programa,Grupo,Materia,ModuloMateria,Gestion,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            TASEstudianteInterfaceClient TASEstudiante = new TASEstudianteInterfaceClient();
            List<TASEstudianteEntity> ltTASEstudiante = TASEstudiante.WebTASEstudianteSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASEstudiante = ltTASEstudiante.Where(x => x.Grupo == Grupo && x.Programa == Programa).ToList();

            ViewBag.SubCompania = SubCompania;
            ViewBag.Programa = Programa;
            ViewBag.Grupo = Grupo;
            ViewBag.Materia = Materia;
            ViewBag.ModuloMateria = ModuloMateria;
            ViewBag.Gestion = Gestion;

            return View(ltTASEstudiante);
        }

        public ActionResult TASRegistroNotasEstudianteEditar(string NombreEstudiante, string CodigoTASEstudiante, string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion)
        {
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar(SubCompania,Programa,Grupo,Materia,ModuloMateria,Gestion,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudiante = new TASRegistroNotasEstudianteEntity();
            List<TASRegistroNotasEstudianteEntity> ltTASRegistroNotasEstudiante = setTASRegistroNotas.ltTASRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == CodigoTASEstudiante).ToList();

            if (ltTASRegistroNotasEstudiante.Count > 0)
            {
                //Seleccionamos la entidad
                etTASRegistroNotasEstudiante = ltTASRegistroNotasEstudiante.First();
            }
            else
            {
                //Inicializamos la entidad y la guardamos
                etTASRegistroNotasEstudiante = new TASRegistroNotasEstudianteEntity {
                    SubCompania = SubCompania,
                    Programa = Programa,
                    Grupo = Grupo,
                    Materia = Materia,
                    ModuloMateria = ModuloMateria,
                    Gestion = Gestion,
                    CodigoTASEstudiante = CodigoTASEstudiante,
                    NombreEstudiante = NombreEstudiante,
                    Progreso1P = "",
                    Progreso2P = "",
                    Dominio1D = "",
                    Dominio2D = "",
                    CalificacionNF = "",
                    CalificacionRE = "",
                    AplicaRecuperatorio = false,
                    NotaLiteral = "",
                    UEstado = TASRegistroNotasService.Estado.Added
                };

                //ltTASRegistroNotasEstudiante.Add(etTASRegistroNotasEstudiante);
                
            }

            ViewBag.ltTASRegistroNotas = setTASRegistroNotas.ltTASRegistroNotas.First();

            return PartialView(etTASRegistroNotasEstudiante);
        }
        [HttpPost]
        public ActionResult TASRegistroNotasEstudianteEditar(TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudianteForm)
        {
            long NumError = 0;
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar(etTASRegistroNotasEstudianteForm.SubCompania, etTASRegistroNotasEstudianteForm.Programa, etTASRegistroNotasEstudianteForm.Grupo, etTASRegistroNotasEstudianteForm.Materia, etTASRegistroNotasEstudianteForm.ModuloMateria, etTASRegistroNotasEstudianteForm.Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            List<TASRegistroNotasEstudianteEntity> ltTASRegistroNotasEstudiante = setTASRegistroNotas.ltTASRegistroNotasEstudiante.Where(x => x.CodigoTASEstudiante == etTASRegistroNotasEstudianteForm.CodigoTASEstudiante).ToList();

            if (ltTASRegistroNotasEstudiante.Count > 0)
            {
                TASRegistroNotasEstudianteEntity etTASRegistroNotasEstudiante = ltTASRegistroNotasEstudiante.First();
                etTASRegistroNotasEstudiante.Progreso1P = etTASRegistroNotasEstudianteForm.Progreso1P.Substring(11);
                etTASRegistroNotasEstudiante.Dominio1D = etTASRegistroNotasEstudianteForm.Dominio1D;
                etTASRegistroNotasEstudiante.CalificacionNF = "";                

                etTASRegistroNotasEstudiante.UEstado = TASRegistroNotasService.Estado.Modified;
            }
            else
            {
                setTASRegistroNotas.ltTASRegistroNotasEstudiante.Add(new TASRegistroNotasEstudianteEntity {
                    SubCompania = etTASRegistroNotasEstudianteForm.SubCompania,
                    Programa = etTASRegistroNotasEstudianteForm.Programa,
                    Grupo = etTASRegistroNotasEstudianteForm.Grupo,
                    Materia = etTASRegistroNotasEstudianteForm.Materia,
                    ModuloMateria = etTASRegistroNotasEstudianteForm.ModuloMateria,
                    Gestion = etTASRegistroNotasEstudianteForm.Gestion,
                    CodigoTASEstudiante = etTASRegistroNotasEstudianteForm.CodigoTASEstudiante,
                    NombreEstudiante = etTASRegistroNotasEstudianteForm.NombreEstudiante,

                    Progreso1P = etTASRegistroNotasEstudianteForm.Progreso1P.Substring(11),
                    Progreso2P = "",
                    Dominio1D = etTASRegistroNotasEstudianteForm.Dominio1D,
                    Dominio2D = "",
                    CalificacionNF = "",
                    CalificacionRE = "",
                    AplicaRecuperatorio = false,
                    NotaLiteral = "",

                    UEstado = TASRegistroNotasService.Estado.Added
                });
            }

            NumError = TASRegistroNotas.WebGuardar(setTASRegistroNotas,false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            if (NumError == 0)
            {
                return Json(new { success = true, url = Url.Action("TASRegistroNotasEditar", "TASRegistroNotas", new { SubCompania = etTASRegistroNotasEstudianteForm.SubCompania, Programa = etTASRegistroNotasEstudianteForm.Programa, Grupo = etTASRegistroNotasEstudianteForm.Grupo, Materia = etTASRegistroNotasEstudianteForm.Materia, ModuloMateria = etTASRegistroNotasEstudianteForm.ModuloMateria, Gestion = etTASRegistroNotasEstudianteForm.Gestion }) });
                //return RedirectToAction("TASRegistroNotasEditar", new { SubCompania = etTASRegistroNotasEstudianteForm.SubCompania, Programa = etTASRegistroNotasEstudianteForm.Programa, Grupo = etTASRegistroNotasEstudianteForm.Grupo, Materia = etTASRegistroNotasEstudianteForm.Materia, ModuloMateria = etTASRegistroNotasEstudianteForm.ModuloMateria, Gestion = etTASRegistroNotasEstudianteForm.Gestion });
            }
            else
            {
                return RedirectToAction("TASRegistroNotasLista");
            }

        }
        public ActionResult TASRegistroNotasEliminar(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar(SubCompania, Programa, Grupo, Materia, ModuloMateria, Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            return View(setTASRegistroNotas);
        }
        [HttpPost,ActionName("TASRegistroNotasEliminar")]
        public ActionResult TASRegistroNotasEliminarConfirmacion(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            long NumError = 0;
            TASRegistroNotasInterfaceClient TASRegistroNotas = new TASRegistroNotasInterfaceClient();
            TASRegistroNotasSet setTASRegistroNotas = TASRegistroNotas.WebSeleccionar(SubCompania, Programa, Grupo, Materia, ModuloMateria, Gestion, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            NumError = TASRegistroNotas.WebEliminar(setTASRegistroNotas, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            if (NumError == 0)
            {
                return RedirectToAction("TASRegistroNotasLista");
            }
            else
            {
                return RedirectToAction("TASRegistroNotasLista");
            }
            
        }

    }
}