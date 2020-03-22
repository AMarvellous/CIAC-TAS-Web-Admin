using adminlte.TASConfiguracionEncuestaSatisfacInstrService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class TASEncuestaSatisfaccionInstructorDetalle
    {
        public TASConfiguracionEncuestaSatisfaccionInstructorEntity etTASConfiguracionEncuestaSatisfaccionInstructor { get; set; }
        public List<long> ltLinea { get; set; }
        public List<string> ltCriterioEvaluacion { get; set; }
        public List<long> ltGradoSatisfaccion { get; set; }
        public string ObservacionSugerencias { get; set; }
    }
}