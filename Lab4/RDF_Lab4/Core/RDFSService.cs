using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace RDF_Lab4.Core
{
    class RDFSService
    {
        private const string VACANCY_URI = @"https://rabota.ua/company/vacancy/";

        private IGraph _graph;
        private INode Integer => _graph.CreateUriNode("xsd:integer");
        private INode Boolean => _graph.CreateUriNode("xsd:boolean");
        private INode String => _graph.CreateUriNode("xsd:string");
        private INode Date => _graph.CreateUriNode("xsd:date");
        public RDFSService()
        {
            _graph = new Graph();
            _graph.NamespaceMap.AddNamespace("rdf", UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            _graph.NamespaceMap.AddNamespace("rdfs", UriFactory.Create("http://www.w3.org/2000/01/rdf-schema#"));
            _graph.NamespaceMap.AddNamespace("rua", UriFactory.Create("https://rabota.ua/"));
        }

        private void BuildSchema()
        {
            var schemaTriples = new List<Triple>();
            schemaTriples.Add(VacancyClass());
            schemaTriples.Add(ItVacancyClass());
            schemaTriples.Add(JuniorVacancyClass());
            schemaTriples.Add(MiddleVacancyClass());
            schemaTriples.Add(SeniorVacancyClass());
            schemaTriples.AddRange(Props());
            _graph.Assert(schemaTriples);
        }

        private void BuildData(List<Vacancy> vacancyList)
        {
            foreach (var vacancy in vacancyList)
            {
                var currentUri = UriFactory.Create(string.Concat(VACANCY_URI, vacancy.Id));
                var allTriples = new List<Triple>()
                {
                    TripleForClass(vacancy, currentUri),
                    TripleForId(vacancy, currentUri),
                    TripleForName(vacancy, currentUri),
                    TripleForCity(vacancy, currentUri),
                    TripleForDate(vacancy, currentUri),
                    TripleForDescription(vacancy, currentUri),
                    TripleForCompanyName(vacancy, currentUri),
                    TripleForHot(vacancy, currentUri)
                };
                _graph.Assert(allTriples);
            }
        }

        private Triple VacancyClass()
        {
            var vacancyNode = _graph.CreateUriNode("rua:Vacancy");
            var typeNode = _graph.CreateUriNode("rdf:type");
            var classNode = _graph.CreateUriNode("rdfs:Class");
            return new Triple(vacancyNode, typeNode, classNode);
        }

        private Triple ItVacancyClass()
        {
            var vacancyNode = _graph.CreateUriNode("rua:ITVacancy");
            var typeNode = _graph.CreateUriNode("rdfs:subClassOf");
            var classNode = _graph.CreateUriNode("rua:Vacancy");
            return new Triple(vacancyNode, typeNode, classNode);
        }

        private Triple JuniorVacancyClass()
        {
            var vacancyNode = _graph.CreateUriNode("rua:JuniorVacancy");
            var typeNode = _graph.CreateUriNode("rdfs:subClassOf");
            var classNode = _graph.CreateUriNode("rua:ITVacancy");
            return new Triple(vacancyNode, typeNode, classNode);
        }

        private Triple MiddleVacancyClass()
        {
            var vacancyNode = _graph.CreateUriNode("rua:MiddleVacancy");
            var typeNode = _graph.CreateUriNode("rdfs:subClassOf");
            var classNode = _graph.CreateUriNode("rua:ITVacancy");
            return new Triple(vacancyNode, typeNode, classNode);
        }

        private Triple SeniorVacancyClass()
        {
            var vacancyNode = _graph.CreateUriNode("rua:SeniorVacancy");
            var typeNode = _graph.CreateUriNode("rdfs:subClassOf");
            var classNode = _graph.CreateUriNode("rua:ITVacancy");
            return new Triple(vacancyNode, typeNode, classNode);
        }

        private List<Triple> Props()
        {
            var props = new List<Triple>();
            // General
            var domain = _graph.CreateUriNode("rdfs:domain");
            var range = _graph.CreateUriNode("rdfs:range");
            var typeNode = _graph.CreateUriNode("rdf:type");
            var propNode = _graph.CreateUriNode("rdf:Property");
            var vacancy = _graph.CreateUriNode("rua:Vacancy");
            var subPropNode = _graph.CreateUriNode("rdfs:subPropertyOf");
            // Id
            var idNode = _graph.CreateUriNode("rua:Id");
            props.Add(new Triple(idNode, range, Integer));
            props.Add(new Triple(idNode, domain, vacancy));
            props.Add(new Triple(idNode, typeNode, propNode));
            // Title
            var titleNode = _graph.CreateUriNode("rua:Title");
            props.Add(new Triple(titleNode, range, String));
            props.Add(new Triple(titleNode, domain, vacancy));
            props.Add(new Triple(titleNode, typeNode, propNode));
            // Date
            var dateNode = _graph.CreateUriNode("rua:Date");
            props.Add(new Triple(dateNode, range, Date));
            props.Add(new Triple(dateNode, domain, vacancy));
            props.Add(new Triple(dateNode, typeNode, propNode));
            // LongDate
            var dateLongNode = _graph.CreateUriNode("rua:LongDate");
            props.Add(new Triple(dateLongNode, subPropNode, dateNode));
            props.Add(new Triple(dateLongNode, typeNode, propNode));
            // Location
            var locationNode = _graph.CreateUriNode("rua:Location");
            props.Add(new Triple(locationNode, range, String));
            props.Add(new Triple(locationNode, domain, vacancy));
            props.Add(new Triple(locationNode, typeNode, propNode));
            // Description
            var descNode = _graph.CreateUriNode("rua:Description");
            props.Add(new Triple(descNode, range, String));
            props.Add(new Triple(descNode, domain, vacancy));
            props.Add(new Triple(descNode, typeNode, propNode));
            // Company
            var companyNode = _graph.CreateUriNode("rua:Company");
            props.Add(new Triple(companyNode, range, String));
            props.Add(new Triple(companyNode, domain, vacancy));
            props.Add(new Triple(companyNode, typeNode, propNode));
            // Hot
            var hotNode = _graph.CreateUriNode("rua:Hot");
            props.Add(new Triple(hotNode, range, Boolean));
            props.Add(new Triple(hotNode, domain, vacancy));
            props.Add(new Triple(hotNode, typeNode, propNode));

            return props;
        }

        private string ResolveClass(string name)
        {
            if (name.Contains("junior"))
                return "rua:JuniorVacancy";
            else if (name.Contains("middle"))
                return "rua:MiddleVacancy";
            else if (name.Contains("senior"))
                return "rua:SeniorVacancy";
            else
                return "rua:ITVacancy";
        }

        private Triple TripleForClass(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var typeNode = _graph.CreateUriNode("rdf:type");
            var classNode = _graph.CreateUriNode(ResolveClass(vacancy.Name.ToLower()));
            return new Triple(vacancyUriNode, typeNode, classNode);
        }

        private Triple TripleForId(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var idPredicate = _graph.CreateUriNode("rua:Id");
            var idLiteral = _graph.CreateLiteralNode(vacancy.Id.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypePositiveInteger));
            return new Triple(vacancyUriNode, idPredicate, idLiteral);
        }

        private Triple TripleForName(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("rua:Title");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.Name);
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForDate(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("rua:LongDate");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.Date.ToString(),
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDate));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForCity(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("rua:Location");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.CityName,
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForDescription(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("rua:Description");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.ShortDescription,
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForCompanyName(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("rua:Company");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.CompanyName,
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
        }

        private Triple TripleForHot(Vacancy vacancy, Uri vacancyUri)
        {
            var vacancyUriNode = _graph.CreateUriNode(vacancyUri);
            var namePred = _graph.CreateUriNode("rua:Hot");
            var nameLiteral = _graph.CreateLiteralNode(vacancy.Hot.ToString(),
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeBoolean));
            return new Triple(vacancyUriNode, namePred, nameLiteral);
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

        public void ShowGraph()
        {
            ShowTriples(_graph.Triples);
        }

        public void WriteSchema()
        {
            var writer = new RdfXmlWriter();
            string path = @"E:\3 курс\WeB\Lab4\RDF_Lab4\StaticFiles\rabotaSchema.rdf";
            writer.Save(_graph, path);
        }

        public void Act(List<Vacancy> vacancyList = null)
        {
            BuildSchema();
            if (vacancyList != null)
                BuildData(vacancyList);
        }
    }
}
