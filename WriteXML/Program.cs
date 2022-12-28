using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WriteXML
{
    class Program
    {
        static void Main(string[] args)
        {
            string FilePath =@"C: \Users\Ruth\Downloads\clients.xml";
            WriteXml(FilePath);
            ReadXml(FilePath);
        }
     public static void WriteXml(string FilePath)
        {
            List<client> AllClients = new List<client>();
            using (SqlConnection conn = new SqlConnection(getConnectionString()))
            {
                string query = @"SELECT  [Id_Client]
      ,[hebrew_name]
      ,[english_name]
      ,[born_date]
      ,[city_code]
      ,[bank]
      ,[branch]
      ,[account_number]
      ,[identity_card]
  FROM [test].[dbo].[clients]";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.CommandTimeout = 1500;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader != null)
                {

                    // יצירת הרשימה מפקודת ה-SQL
                    while (reader.Read())
                    {
                        AllClients.Add(new client()
                        {
                            id = String.Format("{0}", reader["Id_Client"]),
                            hebrew_name = String.Format("{0}", reader["hebrew_name"]),
                            english_name = String.Format("{0}", reader["english_name"]),
                            account_number = String.Format("{0}", reader["account_number"]),
                            bank = String.Format("{0}", reader["bank"]),
                            branch = String.Format("{0}", reader["branch"]),
                            city_code = String.Format("{0}", reader["city_code"]),
                            identity_card = String.Format("{0}", reader["identity_card"]),
                        });
                    }
                }
                if (AllClients.Count > 0)

                // שמירת הרשימה לקובץ
                {
                    var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                    var serializer = new XmlSerializer(AllClients.GetType());
                    var settings = new XmlWriterSettings()
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = new System.Text.UTF8Encoding(false)
                    };

                    using (var stream = new StringWriterUtf8())
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        serializer.Serialize(writer, AllClients, emptyNamespaces);
                        File.AppendAllText(FilePath, stream.ToString());
                    }

                }
                conn.Close();
            }
        }
        public static void ReadXml(string FilePath)
        {
            var xmlStr = File.ReadAllText( FilePath);
            var str = XElement.Parse(xmlStr);
            var result = str.Elements("client").Select(x => x.Element("hebrew_name").Value).ToList();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
        private class StringWriterUtf8 : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
        private static string getConnectionString()
        {
            return "Data Source=localhost;Initial Catalog=test;Integrated Security=True";
        }
    }
}
