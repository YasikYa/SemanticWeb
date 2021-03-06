﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

namespace XMLParsing.XML
{
    class XmlEditor : IDocumentEditor
    {
        private readonly string _xmlPath;
        public XmlEditor(string documentPath) => _xmlPath = documentPath;

        private XmlDocument _document;
        private XmlDocument Doc
        {
            get
            {
                if(_document == null)
                {
                    _document = LoadDocument();
                }
                return _document;
            }
        }

        public void AddAttribute()
        {
            var vacancyElements = Doc.GetElementsByTagName("Vacancy");
            var nextId = 1;
            foreach(XmlElement vacancy in vacancyElements)
            {
                vacancy.SetAttribute("customId", nextId.ToString());
                nextId++;
            }
        }

        public void AddComplexTypeElement()
        {
            var vacancyElements = Doc.GetElementsByTagName("Vacancy");
            foreach(XmlElement vacancy in vacancyElements)
            {
                vacancy.AppendChild(CreateTimeStamp());
            }
        }

        public void AddTextValue()
        {
            var vacancyElements = Doc.GetElementsByTagName("Vacancy");
            foreach(XmlElement vacancy in vacancyElements)
            {
                var dateNode = vacancy.ChildNodes.Item(1);
                var date = DateTime.Parse(dateNode.InnerText);
                dateNode.InnerText = date.ToShortDateString();
            }
        }

        private XmlDocument LoadDocument()
        {
            var document = new XmlDocument();
            document.Load(_xmlPath);
            return document;
        }

        public void SaveDocument()
        {
            var newFile = Path.GetFileNameWithoutExtension(_xmlPath) + "Modify" + ".xml";
            var dir = Path.GetDirectoryName(_xmlPath);
            var newPath = Path.Combine(dir, newFile);
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            using (var writer = XmlWriter.Create(newPath, xmlWriterSettings))
            {
                Doc.WriteTo(writer);
            }
            _document = null; 
        }

        private XmlElement CreateTimeStamp()
        {
            var dateElement = Doc.CreateElement("TimeStamp");
            dateElement.InnerText = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))
                .TotalSeconds).ToString();
            dateElement.SetAttribute("timeUnit", "seconds");
            return dateElement;
        }

        public void AddComplex()
        {
            var vacancyElements = Doc.GetElementsByTagName("Vacancy");
            var vacancy = vacancyElements.Item(vacancyElements.Count - 1).Clone();
            foreach(XmlAttribute id in vacancy.Attributes)
            {
                int currId = int.Parse(id.InnerText);
                id.InnerText = (++currId).ToString();
            }
            var vacancyChilds = vacancy.ChildNodes;
            foreach(XmlElement field in vacancyChilds)
            {
                field.InnerText = "Test";
            }

            var topList = Doc.GetElementsByTagName("ArrayOfVacancy").Item(0);
            topList.AppendChild(vacancy);
        }

        public void RemoveAttribute()
        {
            var vacancyElements = Doc.GetElementsByTagName("Vacancy");
            foreach (XmlElement vacancy in vacancyElements)
            {
                vacancy.RemoveAttribute("Hot");
            }
        }
    }
}
