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

        public string conn = ConfigurationManager.AppSettings["connString"];
        public static SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["connString"]);
        public static string TemplateName = ConfigurationManager.AppSettings["TemplateName"];
        public static string outPath = ConfigurationManager.AppSettings["Path"];
        static void Main(string[] args)
        {
            try
            {
                sqlConn.Open();
                Load();
                /*
                sqlConn.Open();
                ListInstanceByNameResponse listInstanceByNameResponse =
                    new Ac4yObjectObjectService(sqlConn).ListInstanceByName(
                        new ListInstanceByNameRequest() { TemplateName = TemplateName }
                    );


                foreach (var element in listInstanceByNameResponse.Ac4yObjectHomeList)
                {
                    string xml = serialize(element, typeof(Ac4yObjectHome));

                    writeOut(xml, element.SimpledHumanId + "@" + element.TemplateHumanId + "@Ac4yObjectHome", outPath);
                }*/
            }
            catch(Exception exception)
            {
                _naplo.Error(exception.StackTrace);
            }

        }


        public static void writeOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".xml", text);

        }

        public static string serialize(Object taroltEljaras, Type anyType)
        {
            XmlSerializer serializer = new XmlSerializer(anyType);
            var xml = "";

            using (var writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    serializer.Serialize(writer, taroltEljaras);
                    xml = writer.ToString(); // Your XML
                }
            }
            //System.IO.File.WriteAllText(path, xml);

            return xml;
        }

        public static void Load()
        {
            Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(sqlConn);

            try
            {
                string[] files =
                    Directory.GetFiles(outPath, "*.xml", SearchOption.TopDirectoryOnly);

                Console.WriteLine(files.Length);

                foreach (var _file in files)
                {
                    string _filename = Path.GetFileNameWithoutExtension(_file);
                    Console.WriteLine(_filename);

                    string xml = readIn(_filename, outPath);

                    Ac4yObjectHome tabla = (Ac4yObjectHome)deser(xml, typeof(Ac4yObjectHome));

                    SetByNamesResponse response = ac4YObjectObjectService.SetByNames(
                        new SetByNamesRequest() { TemplateName = TemplateName, Name = tabla.SimpledHumanId }
                        );
                    
                }
            }
            catch (Exception _exception)
            {
                Console.WriteLine(_exception.Message);
            }
        }

        public static Object deser(string xml, Type anyType)
        {
            Object result = null;

            XmlSerializer serializer = new XmlSerializer(anyType);
            using (TextReader reader = new StringReader(xml))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static string readIn(string fileName, string templatesFolder)
        {

            string textFile = outPath + fileName + ".xml";

            string text = File.ReadAllText(textFile);

            return text;


        }
    }
}
