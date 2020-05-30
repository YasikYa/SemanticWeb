using Newtonsoft.Json;
using RDF_Lab4.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RDF_Lab4
{
    class Program
    {
        const string JSON_PATH = @"E:\3 курс\WeB\Lab4\RDF_Lab4\StaticFiles\rabota.json";
        const string xmlPath = @"E:\3 курс\WeB\Lab4\RDF_Lab4\StaticFiles\rabota.xml";

        public static List<Vacancy> Current { get; set; }

        static void Main(string[] args)
        {
            var allVacancyTask = ApiCall();
            allVacancyTask.Wait();
            Current = allVacancyTask.Result.ToList();
            RDFSBuild(Current);
            Step();
            SPARQL();
            Console.ReadLine();
        }

        public static void RDFBuild(List<Vacancy> vacancyList)
        {
            var service = new RdfService();
            if (!SkipLoad())
            {
                service.Do(vacancyList);
                Step();
                service.ShowOnlyEnglish();
                Step();
            }
            if (service.Validate())
                Console.WriteLine("Valid");
            else
                Console.WriteLine("Not valid");
        }

        public static void RDFSBuild(List<Vacancy> vacancyList)
        {
            var service = new RDFSService();
            service.Act(vacancyList);
            service.ShowGraph();
            service.WriteSchema();
        }

        public static void SPARQL()
        {
            var service = new QueryService();
            service.Load(@"E:\3 курс\WeB\Lab4\RDF_Lab4\StaticFiles\rabotaSchema.rdf");
            service.Count();
            Step();
            service.OneName();
            Step();
            service.AllSenior();
            Step();
            service.OnlyTitle();
            Step();
            service.SubClasses();
            Step();
            service.OrderByCompany();
            Step();
            service.ByCompanyName(MostPopular());
        }

        public static string MostPopular()
        {
            var result = Current.Select(v => v.CompanyName)
                .Select(n => new { Name = n, Count = Current.Where(v => v.CompanyName == n).Count() })
                .OrderByDescending(r => r.Count).First();

            return result.Name;
        }

        public static bool SkipLoad()
        {
            Console.WriteLine("Skip Load? y | n");
            var answer = Console.ReadLine();
            Console.Clear();
            return answer.Equals("y", StringComparison.OrdinalIgnoreCase);
        }

        public static void Step()
        {
            Console.ReadLine();
            Console.Clear();
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
                    using (var streamWriter = new StreamWriter(File.Create(JSON_PATH)))
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

        private class Root
        {
            public List<Vacancy> Documents { get; set; }
        }
    }
}
