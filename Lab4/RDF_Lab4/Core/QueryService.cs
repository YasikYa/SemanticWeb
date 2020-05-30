using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;
using VDS.RDF.Query.Datasets;

namespace RDF_Lab4.Core
{
    public class QueryService
    {
        private IGraph _graph;
        private LeviathanQueryProcessor _processor;
        private SparqlQueryParser _parser;
        private NamespaceMapper _prefixes;
        public QueryService()
        {
            _graph = new Graph();
            _parser = new SparqlQueryParser();
            var ds = new InMemoryDataset(_graph);
            _processor = new LeviathanQueryProcessor(ds);

            _prefixes = new NamespaceMapper(true);
            _prefixes.AddNamespace("rua", UriFactory.Create("https://rabota.ua/"));
            _prefixes.AddNamespace("rdf", UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            _prefixes.AddNamespace("rdfs", UriFactory.Create("http://www.w3.org/2000/01/rdf-schema#"));
        }

        public void Count()
        {
            var queryStr = "SELECT (COUNT(*) as ?Triples) WHERE {?s ?p ?o}";

            var query = _parser.ParseFromString(queryStr);
            Console.WriteLine(query.ToString());
            var result = _processor.ProcessQuery(query) as SparqlResultSet;
            ShowResult(result);
        }

        public void OneName()
        {
            var queryBuilder = QueryBuilder
                .Select(new[] { "title" })
                .Where(b =>
                {
                    b.Subject<IBlankNode>("").PredicateUri($"rua:Title").Object("title");
                })
                .Limit(1);
            queryBuilder.Prefixes = _prefixes;
            Console.WriteLine(queryBuilder.BuildQuery().ToString());
            var result = _processor.ProcessQuery(queryBuilder.BuildQuery()) as SparqlResultSet;
            ShowResult(result);
        }

        public void AllSenior()
        {
            var sub = "subject";
            var predicate = "predicate";
            var obj = "object";
            var queryBuilder = QueryBuilder
                .Select(new[] { predicate, obj })
                .Where(b =>
                {
                    b.Subject(sub)
                    .PredicateUri("rdf:type")
                    .Object(UriFactory.Create(string.Concat(_prefixes.GetNamespaceUri("rua"), "SeniorVacancy")));
                    b.Subject(sub).Predicate(predicate).Object(obj);
                });
            queryBuilder.Prefixes = _prefixes;
            Console.WriteLine(queryBuilder.BuildQuery().ToString());
            var result = _processor.ProcessQuery(queryBuilder.BuildQuery()) as SparqlResultSet;
            ShowResult(result);
        }

        public void OnlyTitle()
        {
            var title = "title";
            var queryBuilder = QueryBuilder
                .Select(new[] { title })
                .Where(b =>
                {
                    b.Subject<IBlankNode>("")
                    .PredicateUri("rua:Title")
                    .Object(title);
                });
            queryBuilder.Prefixes = _prefixes;
            Console.WriteLine(queryBuilder.BuildQuery().ToString());
            var result = _processor.ProcessQuery(queryBuilder.BuildQuery()) as SparqlResultSet;
            ShowResult(result);
        }

        public void SubClasses()
        {
            var subClass = "subclass";
            var queryBuilder = QueryBuilder
                .Select(new[] { subClass })
                .Where(g =>
                {
                    g.Subject(subClass).PredicateUri("rdfs:subClassOf")
                    .Object(UriFactory.Create(string.Concat(_prefixes.GetNamespaceUri("rua"), "ITVacancy")));
                });
            queryBuilder.Prefixes = _prefixes;
            Console.WriteLine(queryBuilder.BuildQuery().ToString());
            var result = _processor.ProcessQuery(queryBuilder.BuildQuery()) as SparqlResultSet;
            ShowResult(result);
        }

        public void OrderByCompany()
        {
            var title = "title";
            var company = "company";
            var queryBuilder = QueryBuilder
                .Select(new[] { title })
                .Where(b =>
                {
                    b.Subject<IBlankNode>("")
                    .PredicateUri("rua:Title")
                    .Object(title);
                    b.Subject<IBlankNode>("")
                    .PredicateUri("rua:Company").Object(company);
                })
                .Limit(5).OrderBy(company);
            queryBuilder.Prefixes = _prefixes;
            Console.WriteLine(queryBuilder.BuildQuery().ToString());
            var result = _processor.ProcessQuery(queryBuilder.BuildQuery()) as SparqlResultSet;
            ShowResult(result);
        }

        public void ByCompanyName(string company)
        {
            var pred = "pred";
            var obj = "value";
            var queryBuilder = QueryBuilder
                .Select(new[] { pred, obj })
                .Where(b =>
                {
                    b.Subject<IBlankNode>("").PredicateUri("rua:Company").Object(_graph.CreateLiteralNode(company, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString)));
                    b.Subject<IBlankNode>("").Predicate(pred).Object(obj);
                });
            queryBuilder.Prefixes = _prefixes;
            Console.WriteLine(queryBuilder.BuildQuery().ToString());
            var result = _processor.ProcessQuery(queryBuilder.BuildQuery()) as SparqlResultSet;
            ShowResult(result);
        }

        public void Load(string path)
        {
            var parser = new RdfXmlParser();
            parser.Load(_graph, path);
        }

        private void ShowResult(SparqlResultSet resultSet)
        {
            foreach (SparqlResult res in resultSet)
            {
                Console.WriteLine(res.ToString());
            }
        }
    }
}
