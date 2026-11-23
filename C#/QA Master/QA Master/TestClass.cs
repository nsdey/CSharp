using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QA_Master
{
    public class TestClass
    {
        public ObservableCollection<string>? Workflow { get; set; } = null;
        public string? Name { get; set; } = null;

        public string GetXML()
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TestClass));

            MemoryStream ms = new MemoryStream();
            StringWriter stringwriter = new StringWriter();
            xmlSerializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }
    }
}
