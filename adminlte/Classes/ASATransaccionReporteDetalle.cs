using adminlte.ASAPreguntaService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class ASATransaccionReporteDetalle
    {
        public long NroPregunta { get; set; }
        public long Opcion { get; set; }
        public string Respuesta { get; set; }
        public bool Correcto { get; set; }
        public string TipoPregunta { get; set; }
        public long OpcionUsuario { get; set; }
    }
}