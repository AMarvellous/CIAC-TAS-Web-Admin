using adminlte.TASMaterialProgramaAnaliticoService;
using adminlte.TASProgramaAnaliticoService;
using adminlte.TASTransaccionProgramaAnaliticoService;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class TASProgramaAnaliticoController : BaseController
    {
        // GET: TASProgramaAnalitico
        public ActionResult TASProgramaAnaliticoGestionar()
        {
            TASTransaccionProgramaAnaliticoInterfaceClient TASTransaccionProgramaAnalitico = new TASTransaccionProgramaAnaliticoInterfaceClient();
            List<TASTransaccionProgramaAnaliticoEntity> ltTASTransaccionProgramaAnalitico = TASTransaccionProgramaAnalitico.WebTASTransaccionProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASTransaccionProgramaAnalitico = ltTASTransaccionProgramaAnalitico.Where(x => x.InstructorNroLicencia == (string)Session["Usuario"]).ToList();
            
            return View(ltTASTransaccionProgramaAnalitico);
        }

        public ActionResult TASProgramaAnaliticoVisualizar(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            TASProgramaAnaliticoInterfaceClient TASProgramaAnalitco = new TASProgramaAnaliticoInterfaceClient();
            List<TASProgramaAnaliticoEntity> ltTASProgramaAnalitico = TASProgramaAnalitco.WebTASProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASProgramaAnaliticoEntity etTASProgramaAnalitico = ltTASProgramaAnalitico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion && x.InstructorNroLicencia == InstructorNroLicencia).FirstOrDefault();
            
            return View(etTASProgramaAnalitico);
        }

        public ActionResult TASProgramaAnaliticoDescargar(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            
            long NumError = 0;
            string MapPath = "";
            string FileDownloadName = "";
            CrystalDecisions.Shared.ExportFormatType ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            string ContentType = string.Empty;
            MapPath = "~/Reports/TASProgramaAnalitico/TASProgramaAnaliticoInstructor.rpt";
            FileDownloadName = "TASProgramaAnalitico_" + DateTime.Now + ".pdf";
            ExportFormat = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            ContentType = "application/pdf";
            TASProgramaAnaliticoInterfaceClient TASProgramaAnalitco = new TASProgramaAnaliticoInterfaceClient();
            
            try
            {
                //MapPath = "~/Reports/CDComanda/CDTTransaccion.rpt";
                //FileDownloadName = "CDTTransaccion" + DateTime.Now + ".xls";

                ReportDocument report = new ReportDocument();

                List<TASProgramaAnaliticoEntity> ltTASProgramaAnalitico = TASProgramaAnalitco.WebTASProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                ltTASProgramaAnalitico  = ltTASProgramaAnalitico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion && x.InstructorNroLicencia == InstructorNroLicencia).ToList();
                
                report.Load(Server.MapPath(MapPath));
                report.SetDataSource(ltTASProgramaAnalitico);
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
                    return RedirectToAction("TASProgramaAnaliticoGestionar");
                }



            }
            catch (Exception e)
            {
                return RedirectToAction("TASProgramaAnaliticoGestionar");
            }
        }
        public ActionResult TASMaterialProgramaAnaliticoLista()
        {
            //Vista de todos los programas analitico ASIGNADOS, PROGRESO y CERRADOS para subir Material
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            List<TASMaterialProgramaAnaliticoEntity> ltTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebTASMaterialProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            ltTASMaterialProgramaAnalitico = ltTASMaterialProgramaAnalitico.Where(x => x.InstructorNroLicencia == (string)Session["Usuario"]).ToList();

            return View(ltTASMaterialProgramaAnalitico);
        }

        public ActionResult TASMaterialProgramaAnaliticoSubirMaterial(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            List<TASMaterialProgramaAnaliticoEntity> ltTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebTASMaterialProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialProgramaAnaliticoEntity etTASMaterialProgramaAnalitico = ltTASMaterialProgramaAnalitico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion && x.InstructorNroLicencia == InstructorNroLicencia).FirstOrDefault();

            return View(etTASMaterialProgramaAnalitico);
        }

        public ActionResult TASMaterialProgramaAnaliticoDescargarMaterial(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            List<TASMaterialProgramaAnaliticoEntity> ltTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebTASMaterialProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialProgramaAnaliticoEntity etTASMaterialProgramaAnalitico = ltTASMaterialProgramaAnalitico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion && x.InstructorNroLicencia == InstructorNroLicencia).FirstOrDefault();

            return View(etTASMaterialProgramaAnalitico);
        }
        [HttpPost]
        public ActionResult TASMaterialProgramaAnaliticoSubirMaterial(HttpPostedFileBase[] FileUpload,string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            long NumError = 0;
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            TASMaterialProgramaAnaliticoSet setTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebSeleccionar((string)Session["SesionSubCompania"],Programa,Grupo,Materia,ModuloMateria,Gestion,InstructorNroLicencia,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialProgramaAnaliticoEntity etTASMaterialProgramaAnalitico = setTASMaterialProgramaAnalitico.ltTASMaterialProgramaAnalitico.First();
            
            etTASMaterialProgramaAnalitico.UEstado = TASMaterialProgramaAnaliticoService.Estado.Modified;
            etTASMaterialProgramaAnalitico.Estado = "Progreso";

            try
            {
                int Cont = 1;
                //Ensure model state is valid  
                if (ModelState.IsValid)
                {   //iterating through multiple file collection   
                    foreach (HttpPostedFileBase file in FileUpload)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            var InputFileName = Path.GetFileName(file.FileName);
                            var PathFile = Server.MapPath("~/Uploads/TASMaterial/") + Programa + "/" + Grupo + "/" + Materia + "/" + ModuloMateria + "/" + Gestion + "/" + InstructorNroLicencia + "/";
                            if (!Directory.Exists(PathFile))
                            {
                                Directory.CreateDirectory(PathFile);
                            }

                            var ServerSavePath = Path.Combine(PathFile + InputFileName);
                            //Save file to server folder  
                            file.SaveAs(ServerSavePath);
                            //Save in Database
                            typeof(TASMaterialProgramaAnaliticoEntity).GetProperty("Archivo" + Cont).SetValue(etTASMaterialProgramaAnalitico, PathFile + InputFileName, null);
                            Cont++;
                        }

                    }

                    NumError = TASMaterialProgramaAnalitico.WebGuardar(setTASMaterialProgramaAnalitico,false,(string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                }
            }
            catch (Exception e)
            {
                NumError = 1;
            }
                       

            if (NumError == 0)
            {
                return RedirectToAction("TASMaterialProgramaAnaliticoLista", "TASProgramaAnalitico");
            }
            else
            {
                return View();
            }
        }
        public ActionResult TASMaterialProgramaAnaliticoEditarMaterial(string SubCompania, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            List<TASMaterialProgramaAnaliticoEntity> ltTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebTASMaterialProgramaAnaliticoSeleccionarTodo((string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialProgramaAnaliticoEntity etTASMaterialProgramaAnalitico = ltTASMaterialProgramaAnalitico.Where(x => x.SubCompania == SubCompania && x.Programa == Programa && x.Grupo == Grupo && x.Materia == Materia && x.ModuloMateria == ModuloMateria && x.Gestion == Gestion && x.InstructorNroLicencia == InstructorNroLicencia).FirstOrDefault();

            return View(etTASMaterialProgramaAnalitico);
        }

        [HttpPost]
        public ActionResult TASMaterialProgramaAnaliticoEditarMaterial(HttpPostedFileBase FileUpload1, HttpPostedFileBase FileUpload2, HttpPostedFileBase FileUpload3, HttpPostedFileBase FileUpload4, HttpPostedFileBase FileUpload5, HttpPostedFileBase FileUpload6, HttpPostedFileBase FileUpload7, HttpPostedFileBase FileUpload8, HttpPostedFileBase FileUpload9, HttpPostedFileBase FileUpload10,
            HttpPostedFileBase FileUpload11, HttpPostedFileBase FileUpload12, HttpPostedFileBase FileUpload13, HttpPostedFileBase FileUpload14, HttpPostedFileBase FileUpload15, HttpPostedFileBase FileUpload16, HttpPostedFileBase FileUpload17, HttpPostedFileBase FileUpload18, HttpPostedFileBase FileUpload19, HttpPostedFileBase FileUpload20, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            long NumError = 0;
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            TASMaterialProgramaAnaliticoSet setTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialProgramaAnaliticoEntity etTASMaterialProgramaAnalitico = setTASMaterialProgramaAnalitico.ltTASMaterialProgramaAnalitico.First();

            etTASMaterialProgramaAnalitico.UEstado = TASMaterialProgramaAnaliticoService.Estado.Modified;
            //etTASMaterialProgramaAnalitico.Estado = "Progreso";

            List<FileHttpPostedFileBaseClass> FileUpload = new List<FileHttpPostedFileBaseClass>();
            Action<FileHttpPostedFileBaseClass> add = x => { if (x.HttpPostFile != null) FileUpload.Add(new FileHttpPostedFileBaseClass { HttpPostFile = x.HttpPostFile, Position = x.Position }); };
            add(new FileHttpPostedFileBaseClass {  HttpPostFile = FileUpload1, Position = 1 });
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
                            var PathFile = Server.MapPath("~/Uploads/TASMaterial/") + Programa + "/" + Grupo + "/" + Materia + "/" + ModuloMateria + "/" + Gestion + "/" + InstructorNroLicencia + "/";
                            if (!Directory.Exists(PathFile))
                            {
                                Directory.CreateDirectory(PathFile);
                            }

                            var ServerSavePath = Path.Combine(PathFile + InputFileName);
                            //Save file to server folder  
                            file.HttpPostFile.SaveAs(ServerSavePath);
                            //Save in Database
                            typeof(TASMaterialProgramaAnaliticoEntity).GetProperty("Archivo" + file.Position).SetValue(etTASMaterialProgramaAnalitico, PathFile + InputFileName, null);
                    }

                    }

                    NumError = TASMaterialProgramaAnalitico.WebGuardar(setTASMaterialProgramaAnalitico, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                }
            }
            catch (Exception e)
            {
                NumError = 1;
            }


            if (NumError == 0)
            {
                return RedirectToAction("TASMaterialProgramaAnaliticoLista", "TASProgramaAnalitico");
            }
            else
            {
                return View();
            }
        }
        public ActionResult TASMaterialProgramaAnaliticoDescargarArchivo(string Archivo)
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
                    return RedirectToAction("TASMaterialProgramaAnaliticoLista");
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TASMaterialProgramaAnaliticoLista");
            }
        }
        public ActionResult TASMaterialProgramaAnaliticoEliminarArchivo(string Archivo,string Variable, string Programa, string Grupo, string Materia, string ModuloMateria, string Gestion, string InstructorNroLicencia)
        {
            long NumError = 0;
            TASMaterialProgramaAnaliticoInterfaceClient TASMaterialProgramaAnalitico = new TASMaterialProgramaAnaliticoInterfaceClient();
            TASMaterialProgramaAnaliticoSet setTASMaterialProgramaAnalitico = TASMaterialProgramaAnalitico.WebSeleccionar((string)Session["SesionSubCompania"], Programa, Grupo, Materia, ModuloMateria, Gestion, InstructorNroLicencia, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            TASMaterialProgramaAnaliticoEntity etTASMaterialProgramaAnalitico = setTASMaterialProgramaAnalitico.ltTASMaterialProgramaAnalitico.First();

            etTASMaterialProgramaAnalitico.UEstado = TASMaterialProgramaAnaliticoService.Estado.Modified;
            try
            {
                
                if (System.IO.File.Exists(Archivo))
                {
                    System.IO.File.Delete(Archivo);
                }
                typeof(TASMaterialProgramaAnaliticoEntity).GetProperty(Variable).SetValue(etTASMaterialProgramaAnalitico, "", null);

                NumError = TASMaterialProgramaAnalitico.WebGuardar(setTASMaterialProgramaAnalitico, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                if (NumError == 0)
                {
                    return RedirectToAction("TASMaterialProgramaAnaliticoEditarMaterial", new { SubCompania = (string)Session["SesionSubCompania"], Programa = Programa, Grupo = Grupo, Materia = Materia, ModuloMateria = ModuloMateria, Gestion = Gestion, InstructorNroLicencia = InstructorNroLicencia });
                }
                else
                {
                    return RedirectToAction("TASMaterialProgramaAnaliticoEditarMaterial",new { SubCompania = (string)Session["SesionSubCompania"], Programa = Programa, Grupo = Grupo, Materia = Materia, ModuloMateria = ModuloMateria, Gestion = Gestion, InstructorNroLicencia = InstructorNroLicencia });
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("TASMaterialProgramaAnaliticoLista");
            }
        }
    }
}