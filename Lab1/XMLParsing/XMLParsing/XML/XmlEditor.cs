using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XMLParsing.XML
{
    class XmlEditor : IDocumentEditor
    {
        private readonly string _xmlPath;
        public XmlEditor(string documentPath) => _xmlPath = documentPath;
        
        public void AddAttribute()
        {
            var doc = LoadDocument();
            var vacancyElements = doc.GetElementsByTagName("Vacancy");
            var nextId = 1;
            foreach(XmlElement vacancy in vacancyElements)
            {
                vacancy.SetAttribute("customId", nextId.ToString());
                nextId++;
            }
        }

        public void AddComplexTypeElement()
        {
            throw new NotImplementedException();
        }

        public void AddTextValue()
        {
            throw new NotImplementedException();
        }

        private XmlDocument LoadDocument()
        {
            var document = new XmlDocument();
            document.Load(_xmlPath);
            return document;
        }
    }
}
