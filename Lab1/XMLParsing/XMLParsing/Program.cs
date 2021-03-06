﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Xsl;
using XMLParsing.XML;

namespace XMLParsing
{
    class Program
    {
        private const string SCHEMA_PATH = @"E:\3 курс\WeB\Lab1\XMLParsing\XMLParsing\StaticFiles\schema.xml";
        private const string PATH = @"E:\3 курс\WeB\Lab1\XMLParsing\XMLParsing\StaticFiles\rabota.xml";
        private const string JSON_PATH = @"E:\3 курс\WeB\Lab1\XMLParsing\XMLParsing\StaticFiles\rabota.json";
        private const string XSL_PATH = @"E:\3 курс\WeB\Lab1\XMLParsing\XMLParsing\StaticFiles\Styles.xslt";
        private const string HTML_PATH = @"E:\3 курс\WeB\Lab1\XMLParsing\XMLParsing\StaticFiles\rabota.html";
        static void Main(string[] args)
        {
            XPathMain();
            //GenerateHTML();
            //Editor();
            //Schema();
            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        public static void Schema()
        {
            var mainTask = AsyncMain();
            Console.WriteLine("In progress");
            mainTask.Wait();
        }

        public static void Editor()
        {
            IDocumentEditor editor = new XmlEditor(PATH);
            editor.AddTextValue();
            editor.AddComplexTypeElement();
            editor.AddAttribute();
            editor.RemoveAttribute();
            editor.AddComplex();
            editor.SaveDocument();
        }

        public static void XPathMain()
        {
            IXPathProvider xPath = new QueryProvider(PATH);
            BreakPoint();
            xPath.CountVacancy();
            BreakPoint();
            xPath.SelectAttributes();
            BreakPoint();
            xPath.SelectInfoSingle();
            BreakPoint();
            xPath.SelectOnLength();
            BreakPoint();
            xPath.SelectFirstChild();
            BreakPoint();
            xPath.SelectSecondChild();
            BreakPoint();
            xPath.SelectThirdChild();
            BreakPoint();
            xPath.SelectOnStatus();
            BreakPoint();
            xPath.EveryFifthData();
            BreakPoint();
            xPath.OddCount();
            BreakPoint();
        }

        private static void BreakPoint()
        {
            Console.WriteLine("\nPress any key to next query");
            Console.ReadLine();
            Console.Clear();
        }

        public static async Task AsyncMain()
        {
            ISchemaService schemaService = new VacancyXmlSchema();
            XmlSchema vacancySchema;
            using (StreamWriter writer = File.CreateText(SCHEMA_PATH))
            {
                vacancySchema = schemaService.BuildSchema(writer);
            }
            var entities = await ApiCall().ConfigureAwait(false);
            XmlSerialize(entities.ToList());
            schemaService.ValidateSchema(PATH, vacancySchema);
        }

        public static async Task<IEnumerable<Vacancy>> ApiCall()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.rabota.ua/");

                var request = new HttpRequestMessage(HttpMethod.Get, "vacancy/search?cityId=0&parentId=1");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.SendAsync(request);

                IEnumerable<Vacancy> entities = null;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using(var streamWriter = new StreamWriter(File.Create(JSON_PATH)))
                    {
                        streamWriter.Write(content);
                    }
                    entities = ParseJSON(content);
                }

                return entities;
            }
        }

        public static IEnumerable<Vacancy> ParseJSON(string json)
        {
            var root = JsonConvert.DeserializeObject<Root>(json);
            return root.Documents;
        }

        public static void XmlSerialize<TItem>(TItem item)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TItem));
            using(var stream = File.Create(PATH))
            {
                serializer.Serialize(stream, item);
            }
        }

        public static void GenerateHTML()
        {
            XsltArgumentList args = new XsltArgumentList();
            XslCompiledTransform transformObj = new XslCompiledTransform();
            transformObj.Load(XSL_PATH);

            using(var reader = XmlReader.Create(File.OpenRead(PATH)))
            {
                using(var writer = File.Create(HTML_PATH))
                {
                    transformObj.Transform(reader, args, writer);
                }
            }
        }
    }
}
