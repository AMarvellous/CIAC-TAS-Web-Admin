using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte
{
    public class FileHttpPostedFileBaseClass
    {
        public long Position {get; set; }
        public HttpPostedFileBase HttpPostFile { get; set; }

    }
}