using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RDF_Lab4
{
    public class Vacancy
    {
        [XmlAttribute(AttributeName = "Id", DataType = "int")]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string CityName { get; set; }

        public string ShortDescription { get; set; }

        public string CompanyName { get; set; }

        [XmlAttribute(AttributeName = "Hot", DataType = "boolean")]
        public bool Hot { get; set; }
    }
}
