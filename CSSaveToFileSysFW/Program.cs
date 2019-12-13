using CSAc4yService.Class;
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
        
        public static string conn = ConfigurationManager.AppSettings["connString"];
        //public static SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["connString"]);
        //public static SqlConnection sqlConnXML = new SqlConnection(ConfigurationManager.AppSettings["connStringXML"]);
        public static string TemplateName = ConfigurationManager.AppSettings["TemplateName"];
        public static string outPath = ConfigurationManager.AppSettings["Path"];
        public static string outPathProcess = ConfigurationManager.AppSettings["PathProcess"];
        public static string outPathSuccess = ConfigurationManager.AppSettings["PathSuccess"];
        public static string outPathError = ConfigurationManager.AppSettings["PathError"];

        static void Main(string[] args)
        {
            try
            {
                //GetXmls getXmls = new GetXmls();
                SaveToFileSysFW saveToFileSysFW = new SaveToFileSysFW(conn, TemplateName, outPath, outPathProcess, outPathSuccess, outPathError);

                //saveToFileSysFW.Load();
                saveToFileSysFW.WriteOutAc4yObjectAll();
                //saveToFileSysFW.WriteOutAc4yObjectHome();
                //sqlConnXML.Open();
                /*
                List<SerializationObject> xmls = getXmls.GetXmlsMethod(sqlConn, sqlConnXML, TemplateName);
                foreach(var xml in xmls)
                {
                    saveToFileSysFW.writeOut(xml.xml, xml.fileName, outPath);
                }
                */
            }
            catch(Exception exception)
            {
                _naplo.Error(exception.Message);
            }

        }

    }
}
