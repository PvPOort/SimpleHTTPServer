using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHTTPServer
{
    public class ServiceBase
    {

        public CoreTools.ResponseData print()
        {
            return new CoreTools.ResponseData(CoreTools.EResponseType.ERT_HTML, "<HTML>\n<HEAD>\n<TITLE>Title</TITLE>\n</HEAD>\n<BODY BGCOLOR=\"FFFFFF\">\nOK\n</BODY>\n</HTML>");
        }
    }
}
