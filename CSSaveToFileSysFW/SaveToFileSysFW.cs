﻿using CSAc4yObjectDBCap;
using CSAc4yObjectObjectService.Object;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CSSaveToFileSysFW
{
    public class SaveToFileSysFW
    {
        
        public SqlConnection sqlConn;
        public string sqlConnectionString;
        public string TemplateName;
        public string outPath;
        public string outPathProcess;
        public string outPathSuccess;
        public string outPathError;

        public SaveToFileSysFW() { }

        public SaveToFileSysFW(string newSqlConn, string newTemp, string newOut, string newProc, string newSucc, string newErr)
        {
            sqlConnectionString = newSqlConn;
            TemplateName = newTemp;
            outPath = newOut;
            outPathProcess = newProc;
            outPathSuccess = newSucc;
            outPathError = newErr;

            sqlConn = new SqlConnection(sqlConnectionString);
            sqlConn.Open();
        }

        public void WriteOutAc4yObjectHome()
        {
            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(sqlConn).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = TemplateName }
                );

            foreach (var element in listInstanceByNameResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));

                writeOut(xml, element.SimpledHumanId + "@" + element.TemplateHumanId + "@Ac4yObjectHome", outPath);
            }

        }


        public static void writeOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".xml", text);

        }

        public string serialize(Object taroltEljaras, Type anyType)
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

        public void Load()
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

                    Ac4yObject tabla = (Ac4yObject)deser(xml, typeof(Ac4yObject));

                    SetByNamesResponse response = ac4YObjectObjectService.SetByNames(
                        new SetByNamesRequest() { TemplateName = TemplateName, Name = tabla.SimpledHumanId }
                        );

                    if (response.Result.Code.Equals("1"))
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

        public Object deser(string xml, Type anyType)
        {
            Object result = null;

            XmlSerializer serializer = new XmlSerializer(anyType);
            using (TextReader reader = new StringReader(xml))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public string readIn(string fileName, string path)
        {

            string textFile = path + fileName + ".xml";

            string text = File.ReadAllText(textFile);

            return text;


        }
    }
}
