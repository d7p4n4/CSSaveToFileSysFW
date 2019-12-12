using CSAc4yService.Class;
using CSLibAc4yObjectDBCap;
using CSLibAc4yObjectObjectService;
using d7p4n4Namespace.Final.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CSSaveToFileSysFW
{
    class Program
    {
        private static readonly log4net.ILog _naplo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            try
            {
                GetXmls getXmls = new GetXmls();
                SaveToFileSysFW saveToFileSysFW = new SaveToFileSysFW();
                
                saveToFileSysFW.Load();
                /*
                List<SerializationObject> xmls = getXmls.GetXmlsMethod(sqlConn, sqlConnXML, TemplateName);
                foreach(var xml in xmls)
                {
                    writeOut(xml.xml, xml.fileName, outPath);
                }
                */
            }
            catch(Exception exception)
            {
                _naplo.Error(exception.StackTrace);
            }

        }

    }
}
