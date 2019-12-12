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

        public string conn = ConfigurationManager.AppSettings["connString"];
        public static SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["connString"]);
        public static SqlConnection sqlConnXML = new SqlConnection(ConfigurationManager.AppSettings["connStringXML"]);
        public static string TemplateName = ConfigurationManager.AppSettings["TemplateName"];
        public static string outPath = ConfigurationManager.AppSettings["Path"];
        public static string outPathProcess = ConfigurationManager.AppSettings["PathProcess"];
        public static string outPathSuccess = ConfigurationManager.AppSettings["PathSuccess"];
        public static string outPathError = ConfigurationManager.AppSettings["PathError"];
        static void Main(string[] args)
        {
            try
            {
                GetXmls getXmls = new GetXmls();

                sqlConn.Open();
                sqlConnXML.Open();
                Load();
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

        public static void WriteOutAc4yObjectHome()
        {
            sqlConn.Open();
            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(sqlConn).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = TemplateName }
                );
            
            foreach (var element in listInstanceByNameResponse.Ac4yObjectHomeList)
            {
                string xml = serialize(element, typeof(Ac4yObjectHome));

                writeOut(xml, element.SimpledHumanId + "@" + element.TemplateHumanId + "@Ac4yObjectHome", outPath);
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
                    System.IO.File.Move(outPath + _filename + ".xml", outPathProcess + _filename + ".xml");


                    string xml = readIn(_filename, outPathProcess);

                    Ac4yObjectHome tabla = (Ac4yObjectHome)deser(xml, typeof(Ac4yObjectHome));

                    SetByNamesResponse response = ac4YObjectObjectService.SetByNames(
                        new SetByNamesRequest() { TemplateName = TemplateName, Name = tabla.SimpledHumanId }
                        );
                    
                    if(response.Result.Code.Equals("1"))
                    {
                        System.IO.File.Move(outPathProcess + _filename + ".xml", outPathSuccess + _filename + ".xml");

                    }
                    else
                    {
                        System.IO.File.Move(outPathProcess + _filename + ".xml", outPathError + _filename + ".xml");

                    }
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

        public static string readIn(string fileName, string path)
        {

            string textFile = path + fileName + ".xml";

            string text = File.ReadAllText(textFile);

            return text;


        }
    }
}
