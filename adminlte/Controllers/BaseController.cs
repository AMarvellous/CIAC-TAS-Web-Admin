using adminlte.AXFSesionService;
using adminlte.AXFUsuarioService;
using adminlte.AXFWebMenuService;
using adminlte.AXFWebVistaService;
using adminlte.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AXFSesionInterfaceClient AXFSesion = new AXFSesionInterfaceClient();
            AXFUsuarioInterfaceClient AXFUsuario = new AXFUsuarioInterfaceClient();
            AXFWebMenuInterfaceClient AXFWebMenu = new AXFWebMenuInterfaceClient();
            AXFWebVistaInterfaceClient AXFWebVista = new AXFWebVistaInterfaceClient();
            List<AXFWebMenuEntity> ltAXFWebMenuLic = new List<AXFWebMenuEntity>();
            long NumError = 0;
            if (Session["Sesion"] != null)
            {
                if (NumError == 0)
                {
                    List<AXFWebVistaEntity> ltAXFWebVista = AXFWebVista.WebAXFWebVistaSeleccionarXSubCompania((string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    List<AXFWebMenuEntity> ltAXFWebMenu = AXFWebMenu.WebAXFWebMenuSeleccionarXUsuarioSubCompania((string)Session["Usuario"], (string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);

                    foreach (AXFWebVistaEntity etAXFWebVista in ltAXFWebVista)
                    {
                        
                            List<AXFWebMenuEntity> ltWebMenuEntity = ltAXFWebMenu.Where(x => etAXFWebVista.WebControlador.Contains(x.WebControlador)).ToList();

                            if (ltWebMenuEntity.Count() > 0)
                            {
                                //AXFWebMenuEntity etWebMenu = ltWebMenuEntity.First();
                                foreach (var WebMenu in ltWebMenuEntity)
                                {
                                    if (!ltAXFWebMenuLic.Contains(WebMenu))
                                    {
                                        ltAXFWebMenuLic.Add(WebMenu);
                                    }
                                }

                            }

                        
                    }
                    //var lt = ltAXFWebMenuLic.OrderBy(x => x.Orden).ToList();
                    ViewBag.ltAXFWebMenu = new List<AXFWebMenuEntity>();

                    List<AXFUsuarioClaseUsuarioEntity> ltAXFUsuarioClaseUsuario = AXFUsuario.WebAXFUsuarioClaseUsuarioSeleccionarKeyAXFUsuario((string)Session["Usuario"], (string)Session["SesionSubCompania"], (string)Session["Sesion"], (string)Session["SesionSubCompania"]);
                    foreach (AXFUsuarioClaseUsuarioEntity etAXFUsuarioClaseUsuario in ltAXFUsuarioClaseUsuario)
                    {
                        ViewBag.Ruta = "~/Imagenes/Gnome-stock_person_steward2.svg.png";
                        if (etAXFUsuarioClaseUsuario.ClaseUsuario == "Administrador")
                        {
                            ViewBag.Ruta = "~/Imagenes/Gnome-stock_person_steward2.svg.png";
                            ViewBag.ltAXFWebMenu = ltAXFWebMenuLic.Where(x => x.Texto0 == "SuperAdmin").OrderBy(x => x.Orden).ToList();
                        }
                        else if (etAXFUsuarioClaseUsuario.ClaseUsuario == "Estudiante")
                        {
                            ViewBag.Ruta = "~/Imagenes/Gnome-stock_person_check2.svg.png";
                        }
                        else if (etAXFUsuarioClaseUsuario.ClaseUsuario == "Instructor")
                        {
                            ViewBag.Ruta = "~/Imagenes/Gnome-stock_person_bureau.svg.png";
                        }

                    }
                    
                    base.OnActionExecuting(filterContext);
                }
                else
                {              
                    filterContext.Result = new RedirectResult("~/Login/Login");
                }

            }
            else
            {
                filterContext.Result = new RedirectResult("~/Login/Login");
            }

        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            //Logging the Exception
            filterContext.ExceptionHandled = true;


            var Result = this.View("Error", new HandleErrorInfo(exception,
                filterContext.RouteData.Values["controller"].ToString(),
                filterContext.RouteData.Values["action"].ToString()));

            //Guardamos Excepcion
            HelperLogClass.GuardarLogExcepcion(filterContext.RouteData.Values["controller"].ToString(), filterContext.RouteData.Values["action"].ToString(), exception.Message, "Exception");
            //UtilMesaAyudaClass.GuardarLogExcepcion(filterContext.RouteData.Values["controller"].ToString(), filterContext.RouteData.Values["action"].ToString(), exception.StackTrace, "Exception");

            filterContext.Result = Result;
        }
    }
}