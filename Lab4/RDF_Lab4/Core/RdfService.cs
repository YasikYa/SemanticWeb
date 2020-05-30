using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Parsing;
using VDS.RDF;
using VDS.RDF.Writing;
using System.IO;

namespace RDF_Lab4.Core
{
    class RdfService
    {
        private const string VACANCY_URI = @"https://rabota.ua/company/vacancy/";
        private const string BASE_URI = @"https://api.rabota.ua/vacancy/search?cityId=0&parentId=1";
        private const string SAVE_TO = @"E:\3 курс\WeB\Lab4\RDF_Lab4\StaticFiles\rabota.rdf";

        private IGraph _graph;

        public RdfService()
        {
            _graph = new Graph();
            _graph.NamespaceMap.AddNamespace("dct", UriFactory.Create("http://purl.org/dc/terms/"));
            _graph.NamespaceMap.AddNamespace("dce", UriFactory.Create("http://purl.org/dc/elements/1.1/"));
        }

        public void Do(List<Vacancy> vacancyList)
        {
            _graph.BaseUri = UriFactory.Create(BASE_URI);

            foreach (var vacancy in vacancyList)
            {
                var currentUri = UriFactory.Create(string.Concat(VACANCY_URI, vacancy.Id));
                var allTriples = new List<Triple>()
                {
                    TripleForId(vacancy, currentUri),
                    TripleForName(vacancy, currentUri),
                    TripleForCity(vacancy, currentUri),
                    TripleForDate(vacancy, currentUri),
                    TripleForDescription(vacancy, currentUri),
                    TripleForCompanyName(vacancy, currentUri)
                };
                _graph.Assert(allTriples);
            }
            ShowGraph(_graph);
            WriteGraph(_graph);
        }

        public void ShowOnlyEnglish()
        {
            var enTriples = _graph.GetTriplesWithPredicate(_graph.CreateUriNode("dct:title"))
                .Where(t => t.Object.ToString().Contains("@en"));
            ShowTriples(enTriples);
            
        }

        public bool Validate()
        {
            var parser = new RdfXmlParser();
            try
            {
                parser.Load(new Graph(), SAVE_TO);
                return true;
            }
            catch(RdfParseException ex)
            {
                Console.WriteLine($"Parsing error: {ex.Message}");
                if(ex.InnerException != null)
                    Console.WriteLine($"Inner message: {ex.InnerException.Message}");
                return false;
            }
        }

        private void ShowTriples(IEnumerable<Triple> triples)
        {
            Console.WriteLine($"TOTAL triples - {triples.Count()}");
            foreach (var triple in triples)
            {
                Console.WriteLine("Subject: " + triple.Subject.ToString());
                Console.WriteLine("Predicate: " + triple.Predicate.ToString());
                Console.WriteLine("Object: " + triple.Object.ToString());
                Console.WriteLine("--------NEXT---------");
            }
        }

        public void ShowGraph(IGraph g)
        {
            ShowTriples(g.Triples);
        }

        public void WriteGraph(IGraph g)
        {
            var writer = new RdfXmlWriter();
            string path = @"E:\3 курс\WeB\Lab4\RDF_Lab4\StaticFiles\rabota.rdf";
            writer.Save(g, path);
        }

        private Triple TripleForId(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var idPredicate = _graph.CreateUriNode("dct:identifier");
            var idLiteral = _graph.CreateLiteralNode(vacancy.Id.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypePositiveInteger));
            return new Triple(vacancyUriNode, idPredicate, idLiteral);
        }

        private Triple TripleForName(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("dct:title");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.Name, IsEn(vacancy.Name)? "en": "ru");
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForDate(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("dce:date");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.Date.ToShortDateString(),
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForCity(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("dce:relation");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.CityName,
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForDescription(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("dct:description");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.ShortDescription,
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForCompanyName(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("dct:issued");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.CompanyName,
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private bool IsEn(string str)
        {
            return _keywords.Any(w => str.ToLower().Contains(w));
        }

        private static List<string> _keywords = new List<string>()
            {
                "front", "junior", "middle", "senior", "engineer", "developer", "manager", "administrator"
            };
    }
}
