using adminlte.TASConfiguracionEncuestaAlumnoInstructorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class TASEncuestaAlumnoInstructorDetalle
    {
        public TASConfiguracionEncuestaAlumnoInstructorEntity etTASConfiguracionEncuestaAlumnoInstructor { get; set; }
        public List<long> ltLinea { get; set; }
        public List<string> ltCriterioEvaluacion { get; set; }
        public List<long> ltGradoSatisfaccion { get; set; }
        public string ObservacionSugerencias { get; set; }
    }
}