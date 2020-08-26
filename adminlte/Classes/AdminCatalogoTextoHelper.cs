using adminlte.AdminCatalogoTextoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class AdminCatalogoTextoHelper
    {
        public decimal GetMontoSugerido(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania)
        {
            decimal MontoSugerido = 0;

            AdminCatalogoTextoInterfaceClient AdminCatalogoTexto = new AdminCatalogoTextoInterfaceClient();
            var AdminCatalogoTextoSet = AdminCatalogoTexto.WebSeleccionar(SubCompania, Texto, strAKASesion, strAKASubCompania);
            if (AdminCatalogoTextoSet.ltAdminCatalogoTexto != null && AdminCatalogoTextoSet.ltAdminCatalogoTexto.Count > 0)
            {
                MontoSugerido = AdminCatalogoTextoSet.ltAdminCatalogoTexto[0].MontoSugerido;
            }

            return MontoSugerido;
        }
    }
}