using CSLibAc4yObjectObjectService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSaveToFileSysFW
{
    class GetXmls
    {
        public List<SerializationObject> GetXmlsMethod(SqlConnection conn, SqlConnection connXML, string templateName)
        {
            List<SerializationObject> xmls = new List<SerializationObject>();
            List<Xmls> guids = new List<Xmls>();

            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(conn).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = templateName }
                );

            string sql = "select GUID, serialization from Ac4yXMLObjects;";
            
            using (SqlConnection connection = connXML)
            {
                SqlCommand command = new SqlCommand(
                    sql, connection);
                    
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine(reader.FieldCount);
                    while (reader.Read())
                    {
                        Xmls xmlsObj = new Xmls() { serialization = reader.GetValue(1).ToString(), guid = reader.GetValue(0).ToString() };
                        guids.Add(xmlsObj);
                    }
                }
            }
            foreach (var tabla in listInstanceByNameResponse.Ac4yObjectHomeList)
            {
                foreach (var xml in guids)
                {
                    if (xml.guid.Equals(tabla.GUID))
                    {
                        SerializationObject serializationObject = new SerializationObject() {
                            fileName = tabla.SimpledHumanId + "@" + templateName,
                            xml = xml.serialization
                        };
                        xmls.Add(serializationObject);
                        break;
                    }
                }
            }

            return xmls;

        }
    }
}
