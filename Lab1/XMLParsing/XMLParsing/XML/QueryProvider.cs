using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLParsing.XML
{
    class QueryProvider : IXPathProvider
    {
        private string _xmlPath;
        private XmlDocument _document;
        private XmlDocument Doc
        {
            get
            {
                if (_document == null)
                    _document = LoadDocument();
                return _document;
            }
        }
        private XmlWriterSettings Settings => new XmlWriterSettings()
        {
            Indent = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            Encoding = Encoding.Unicode
        };
        public QueryProvider(string filePath) => _xmlPath = filePath;

        public void CountVacancy()
        {
            const string query = "count(//Vacancy)";
            LogQuery(query);
            var nav = Doc.CreateNavigator();
            var result = (double)nav.Evaluate(query);
            Console.WriteLine($"Result - {result}");
            
        }

        public void SelectAttributes()
        {
            const string query = "//Vacancy//@Id";
            LogQuery(query);
            var result = Doc.SelectNodes(query);
            foreach(XmlNode node in result)
            {
                Console.WriteLine("Selected - {0}", node.InnerText);
            }
        }

        private XmlDocument LoadDocument()
        {
            var document = new XmlDocument();
            document.Load(_xmlPath);
            return document;
        }

        private void LogResult(XmlNodeList selectedNodes)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
            using (var writer = XmlWriter.Create(Console.Out, xmlWriterSettings))
            {
                foreach (XmlNode node in selectedNodes)
                {
                    node.WriteTo(writer);
                }
            }
        }

        private void LogResult(XmlNode node)
        {
            using(var writer = XmlWriter.Create(Console.Out, Settings))
            {
                node.WriteTo(writer);
            }
        }

        private void LogQuery(string query)
        {
            Console.WriteLine($"Query - {query}");
        }

        public void SelectInfoSingle()
        {
            const string query = "ArrayOfVacancy/Vacancy[last()]/Name";
            LogQuery(query);
            var result = Doc.SelectSingleNode(query);
            LogResult(result);
        }

        public void SelectOnLength()
        {
            const string query = "//Vacancy[contains(CompanyName, ' ')]";
            LogQuery(query);
            LogResult(Doc.SelectNodes(query));
        }

        public void SelectFirstChild()
        {
            const string query = "//Vacancy/Name";
            LogQuery(query);
            LogResult(Doc.SelectNodes(query));
        }

        public void SelectSecondChild()
        {
            SelectChild(2);
        }

        public void SelectThirdChild()
        {
            SelectChild(3);
        }

        private void SelectChild(int index)
        {
            string query = $"//Vacancy/child::*[{index}]";
            LogQuery(query);
            LogResult(Doc.SelectNodes(query));
        }

        public void SelectOnStatus()
        {
            const string query = "//Vacancy[@Id > 7931153 and @Id < 7931253 and contains(Name, 'Senior')]";
            LogQuery(query);
            LogResult(Doc.SelectNodes(query));
        }

        public void EveryFifthData()
        {
            const string queryName = "//Vacancy[position() mod 5 = 0]/Name";
            const string queryCompany = "//Vacancy[position() mod 5 = 0]/CompanyName";
            var query = string.Concat(queryName, " | ", queryCompany);
            LogQuery(query);
            LogResult(Doc.SelectNodes(query));
        }

        public void OddCount()
        {
            const string queryAdd = "//Vacancy[position() mod 2 = 0]/CompanyName";
            const string positionQuery = "//Vacancy/Name";
            var query = string.Concat(positionQuery, " | ", queryAdd);
            LogQuery(query);
            LogResult(Doc.SelectNodes(query));
        }
    }
}
