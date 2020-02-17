using adminlte.TASMaterialDidacticoService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASMaterialDidacticoController : BaseController
    {
        // GET: TASMaterialDidactico
        public ActionResult TASMaterialDidacticoLista()
        {
            TASMaterialDidacticoInterfaceClient TASMaterialDidactico = new TASMaterialDidacticoInterfaceClient();
            List<TASMaterialDidacticoEntity> ltTASMaterialDidactico = TASMaterialDidactico.WebTASMaterialDidacticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASMaterialDidactico = ltTASMaterialDidactico.Where(x => x.InstructorNroLicencia == (string)Session["Usuario"]).ToList();

            return View(ltTASMaterialDidactico);
        }

        public ActionResult TASMaterialDidacticoEditarMaterial(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion)
        {
            string InstructorNroLicencia = (string)Session["Usuario"];
            TASMaterialDidacticoInterfaceClient TASMaterialDidactico = new TASMaterialDidacticoInterfaceClient();
            List<TASMaterialDidacticoEntity> ltTASMaterialDidactico = TASMaterialDidactico.WebTASMaterialDidacticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialDidacticoEntity etTASMaterialDidactico = ltTASMaterialDidactico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion && x.InstructorNroLicencia == InstructorNroLicencia).FirstOrDefault();

            return View(etTASMaterialDidactico);
        }

        [HttpPost]
        public ActionResult TASMaterialDidacticoEditarMaterial(HttpPostedFileBase FileUpload1, HttpPostedFileBase FileUpload2, HttpPostedFileBase FileUpload3, HttpPostedFileBase FileUpload4, HttpPostedFileBase FileUpload5, HttpPostedFileBase FileUpload6, HttpPostedFileBase FileUpload7, HttpPostedFileBase FileUpload8, HttpPostedFileBase FileUpload9, HttpPostedFileBase FileUpload10,
            HttpPostedFileBase FileUpload11, HttpPostedFileBase FileUpload12, HttpPostedFileBase FileUpload13, HttpPostedFileBase FileUpload14, HttpPostedFileBase FileUpload15, HttpPostedFileBase FileUpload16, HttpPostedFileBase FileUpload17, HttpPostedFileBase FileUpload18, HttpPostedFileBase FileUpload19, HttpPostedFileBase FileUpload20,
            HttpPostedFileBase FileUpload21, HttpPostedFileBase FileUpload22, HttpPostedFileBase FileUpload23, HttpPostedFileBase FileUpload24, HttpPostedFileBase FileUpload25, HttpPostedFileBase FileUpload26, HttpPostedFileBase FileUpload27, HttpPostedFileBase FileUpload28, HttpPostedFileBase FileUpload29, HttpPostedFileBase FileUpload30,
            string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia, string Link1, string Link2, string Link3, string Link4, string Link5)
        {
            long NumError = 0;
            TASMaterialDidacticoInterfaceClient TASMaterialDidactico = new TASMaterialDidacticoInterfaceClient();
            TASMaterialDidacticoSet setTASMaterialDidactico = TASMaterialDidactico.WebSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialDidacticoEntity etTASMaterialDidactico = setTASMaterialDidactico.ltTASMaterialDidactico.First();

            etTASMaterialDidactico.Link1 = Link1;
            etTASMaterialDidactico.Link2 = Link2;
            etTASMaterialDidactico.Link3 = Link3;
            etTASMaterialDidactico.Link4 = Link4;
            etTASMaterialDidactico.Link5 = Link5;
            etTASMaterialDidactico.UEstado = TASMaterialDidacticoService.Estado.Modified;
            //etTASMaterialDidactico.Estado = "Progreso";

            List<FileHttpPostedFileBaseClass> FileUpload = new List<FileHttpPostedFileBaseClass>();
            Action<FileHttpPostedFileBaseClass> add = x => { if (x.HttpPostFile != null) FileUpload.Add(new FileHttpPostedFileBaseClass { HttpPostFile = x.HttpPostFile, Position = x.Position }); };
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload1, Position = 1 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload2, Position = 2 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload3, Position = 3 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload4, Position = 4 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload5, Position = 5 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload6, Position = 6 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload7, Position = 7 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload8, Position = 8 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload9, Position = 9 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload10, Position = 10 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload11, Position = 11 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload12, Position = 12 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload13, Position = 13 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload14, Position = 14 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload15, Position = 15 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload16, Position = 16 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload17, Position = 17 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload18, Position = 18 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload19, Position = 19 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload20, Position = 20 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload21, Position = 21 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload22, Position = 22 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload23, Position = 23 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload24, Position = 24 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload25, Position = 25 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload26, Position = 26 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload27, Position = 27 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload28, Position = 28 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload29, Position = 29 });
            add(new FileHttpPostedFileBaseClass { HttpPostFile = FileUpload30, Position = 30 });

            try
            {
                //Ensure model state is valid  
                if (ModelState.IsValid)
                {   //iterating through multiple file collection   
                    foreach (FileHttpPostedFileBaseClass file in FileUpload)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            var InputFileName = Path.GetFileName(file.HttpPostFile.FileName);
                            var PathFile = Server.MapPath("~/Uploads/TASMaterialDidactico/") + Programa + "/" + Grupo + "/" + Materia + "/" + ModuloMateria + "/" + Gestion + "/" + InstructorNroLicencia + "/";
                            if (!Directory.Exists(PathFile))
                            {
                                Directory.CreateDirectory(PathFile);
                            }

                            var ServerSavePath = Path.Combine(PathFile + InputFileName);
                            //Save file to server folder  
                            file.HttpPostFile.SaveAs(ServerSavePath);
                            //Save in Database
                            typeof(TASMaterialDidacticoEntity).GetProperty("Archivo" + file.Position).SetValue(etTASMaterialDidactico, PathFile + InputFileName, null);
                        }

                    }

                    NumError = TASMaterialDidactico.WebGuardar(setTASMaterialDidactico, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                }
            }
            catch (Exception e)
            {
                NumError = 1;
            }


            if (NumError == 0)
            {
                return RedirectToAction("TASMaterialDidacticoLista", "TASMaterialDidactico");
            }
            else
            {
                return View();
            }
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
                    return RedirectToAction("TASMaterialDidacticoLista");
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TASMaterialDidacticoLista");
            }
        }
        public ActionResult TASMaterialDidacticoEliminarArchivo(string Archivo, string Variable, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            long NumError = 0;
            TASMaterialDidacticoInterfaceClient TASMaterialDidactico = new TASMaterialDidacticoInterfaceClient();
            TASMaterialDidacticoSet setTASMaterialDidactico = TASMaterialDidactico.WebSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialDidacticoEntity etTASMaterialDidactico = setTASMaterialDidactico.ltTASMaterialDidactico.First();

            etTASMaterialDidactico.UEstado = TASMaterialDidacticoService.Estado.Modified;
            try
            {

                if (System.IO.File.Exists(Archivo))
                {
                    System.IO.File.Delete(Archivo);
                }
                typeof(TASMaterialDidacticoEntity).GetProperty(Variable).SetValue(etTASMaterialDidactico, "", null);

                NumError = TASMaterialDidactico.WebGuardar(setTASMaterialDidactico, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                if (NumError == 0)
                {
                    return RedirectToAction("TASMaterialDidacticoEditarMaterial", new { SubCompania = (string)Session["SesionSubCompania"], Programa = Programa, Grupo = Grupo, Materia = Materia, ModuloMateria = ModuloMateria, Gestion = Gestion, InstructorNroLicencia = InstructorNroLicencia });
                }
                else
                {
                    return RedirectToAction("TASMaterialDidacticoEditarMaterial", new { SubCompania = (string)Session["SesionSubCompania"], Programa = Programa, Grupo = Grupo, Materia = Materia, ModuloMateria = ModuloMateria, Gestion = Gestion, InstructorNroLicencia = InstructorNroLicencia });
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TASMaterialDidacticoLista");
            }
        }


    }
}