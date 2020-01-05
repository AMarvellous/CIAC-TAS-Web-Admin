using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class ASATransaccionReporte
    {
        public string EstudianteCI { get; set; }
        public long Intento { get; set; }
        public long NroPregunta { get; set; }
        public string Opcion { get; set; }
        public DateTime FechaDoc { get; set; }
        public string GrupoPregunta { get; set; }
        public bool Examen { get; set; }

        public string PreguntaEnunciado { get; set; }
        //public List<ASATransaccionReporteDetalle> ltASATransaccionReporteDetalle { get; set; }
    }
}