using adminlte.AXFUsuarioService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class AXFUsuarioController : BaseController
    {
        // GET: AXFUsuario
        public ActionResult AXFUsuarioEditar()
        {
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFUsuarioSet setAXFUsuario = AXFUsuario.WebSeleccionar((string)Session["Usuario"],(string)Session["SesionSubCompania"],(string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            AXFUsuarioEntity etAXFUsuario = setAXFUsuario.ltAXFUsuario.First();
            etAXFUsuario.Clave = AXFUsuario.WebDesencriptarClave(etAXFUsuario.Clave, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

            //Verificamos si existe una imagen para esta pregunta
            if (etAXFUsuario.Texto0 != "")
            {
                //cargamos imagen
                ViewBag.ExisteImagen = true;
                //var path = etAXFUsuario.Texto0.Substring(0, etAXFUsuario.Texto0.IndexOf("@"));
                ViewBag.PathImagen = etAXFUsuario.Texto0;
            }
            else
            {
                //mostramos el selector de imagenes
                ViewBag.ExisteImagen = false;
            }

            return View(setAXFUsuario);
        }

        [HttpPost]
        public ActionResult AXFUsuarioEditar(AXFUsuarioSet setAXFUsuarioForm, HttpPostedFileBase FileUpload)
        {
            long NumError = 0;
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFUsuarioEntity etAXFUsuarioForm = setAXFUsuarioForm.ltAXFUsuario.First();
            if (etAXFUsuarioForm != null)
            {
                AXFUsuarioSet setAXFUsuario = AXFUsuario.WebSeleccionar((string)Session["Usuario"], (string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                AXFUsuarioEntity etAXFUsuario = setAXFUsuario.ltAXFUsuario.First();
                etAXFUsuario.Nombre = etAXFUsuarioForm.Nombre;
                etAXFUsuario.CorreoElectronico = etAXFUsuarioForm.CorreoElectronico;
                etAXFUsuario.Telefono1 = etAXFUsuarioForm.Telefono1;
                //etAXFUsuario.Clave = AXFUsuario.WebEncriptarClave( etAXFUsuarioForm.Clave, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                etAXFUsuario.Clave = etAXFUsuarioForm.Clave;
                etAXFUsuario.UEstado = AXFUsuarioService.Estado.Modified;

                NumError = AXFUsuario.WebGuardar(setAXFUsuario, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
            }
            else
            {
                NumError = 1;
            }

            if (NumError == 0)
            {
                string DirectorioArchivo = Server.MapPath("~/Uploads/Imagenes/AXFUsuario/");
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
                                AXFUsuarioSet setAXFUsuario = AXFUsuario.WebSeleccionar((string)Session["Usuario"], (string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                                AXFUsuarioEntity etAXFUsuario = setAXFUsuario.ltAXFUsuario.First();
                                etAXFUsuario.Texto0 = NombreCompleto;
                                etAXFUsuario.Clave = AXFUsuario.WebDesencriptarClave(etAXFUsuario.Clave, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                                etAXFUsuario.UEstado = AXFUsuarioService.Estado.Modified;
                                NumError = AXFUsuario.WebGuardar(setAXFUsuario, false, (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                            }
                        }
                    }
                }
            }

            if (NumError == 0)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Index");
            }
        }
    }
}