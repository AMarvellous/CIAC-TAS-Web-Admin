using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class ASATransaccionEstudiante
    {
        public string EstudianteCI { get; set; }
        public long Intento { get; set; }
        public DateTime Fecha { get; set; }
        public bool Examen { get; set; }
    }
}