using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace adminlte.Classes
{
    public class HelperLogClass
    {
        public static void GuardarLogExcepcion(string Controller, string Action, string Message, string Type)
        {
            //Creamos la carpeta
            string PathLog = System.AppDomain.CurrentDomain.BaseDirectory + "LogsErrors\\" + DateTime.Now.ToString("dd-MM-yyyy") + " Log.txt";
            StreamWriter log;

            if (!File.Exists(PathLog))
            {
                (new FileInfo(PathLog)).Directory.Create();
                log = new StreamWriter(PathLog);
            }
            else
            {
                log = File.AppendText(PathLog);
            }

            log.WriteLine("[Fecha]: " + DateTime.Now + "[Tipo]" + Type + " [Excepcion/Evento]: " + Message + " [Clase]: " + Controller + " [Action]: " + Action);
            // Close the stream:
            log.Close();
        }
    }
}