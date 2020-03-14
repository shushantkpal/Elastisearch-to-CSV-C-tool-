using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using CSVTool;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CSVTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                long  elapsed_time;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Config _conf = new Config();
                _conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"Config.json"));
                var node = new Uri(_conf.EsClientAddress);
                string ss = "ELASTIC TO CSV TOOL";
                Console.Title="CSVRiver";
                Console.SetCursorPosition((Console.WindowWidth - ss.Length) / 2, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ss);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ESClientAddress \t:{0}", _conf.EsClientAddress);
                Console.WriteLine("TenanID \t\t:{0}", _conf.TenantID);
                Console.WriteLine("Provided IndexTYPE\t:{0}",_conf.IndexType);
                Console.WriteLine("Provided QUERY JSON ");
                Console.ResetColor();
                var settings = new ConnectionSettings(node).SetTimeout(100000000);             
                //sanitize request
                _conf.Query = Regex.Replace(_conf.Query, @"\s+", string.Empty);
                //removing query:{ to pass the string as RawQuery in Search nest
                _conf.Query = Regex.Replace(_conf.Query,"\"query\":{", string.Empty);
                string Filename = _conf.OutputFilepath;
                _conf.OutputFilepath = Filename;
                var client = new ElasticClient(settings);
                var searchResponse = Search(_conf, client);
                Console.WriteLine(searchResponse.RequestInformation);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Total Doc Count for query is:{0}", searchResponse.Total);
                Console.ResetColor();
                List<Dictionary<string, string>> fields = new List<Dictionary<string, string>>();
                foreach (var DOC in searchResponse.Hits.Select(p => p.Source.ToString()).ToList())
                {
                 var field = JsonConvert.DeserializeObject<Dictionary<string, string>>(DOC);
                 fields.Add(field);
                }
                
                string columnValues = string.Join(",", fields[0].Select(p => p.Key));
                List<string> joinedValues = new List<string>();

                CSVRowValuesList(fields, joinedValues);

                CSVwriter(_conf, columnValues, joinedValues);
                stopwatch.Stop();
                elapsed_time = stopwatch.ElapsedMilliseconds;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("File Successfully Saved........");
                Console.WriteLine("\n\nProvided FileName:\t\t{0}",_conf.OutputFilepath);
                Console.WriteLine("Total ElapsedTime in Seconds\t:{0}",elapsed_time/1000);
                Console.ResetColor();
               
            }

            catch (IOException Ioe)
            {

                Console.WriteLine("\n\nException Occured");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Message: {0}", Ioe.Message);
              
                Console.ResetColor();
            }
            catch (ElasticsearchServerException Ioe)
            {
                Console.WriteLine("\n\nException Occured");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Message: {0}", Ioe.Message);
               
                Console.ResetColor();
            }
            catch (Exception Ioe)
            {
                Console.WriteLine("\n\nException Occured");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Message: {0}", Ioe.Message);
             
                Console.ResetColor();
            }
            finally {
                Console.Read();
            }
           
        }

        private static void CSVRowValuesList(List<Dictionary<string, string>> fields, List<string> joinedValues)
        {
            foreach (var dictvalues in fields)
            {
                string sb2 = string.Join(",", dictvalues.Select(p => p.Value));
                joinedValues.Add(sb2);
            }
        }

        private static void CSVwriter(Config _conf, string columnValues, List<string> joinedValues)
        {
            using (var file = File.CreateText(_conf.OutputFilepath))
            {
                file.WriteLine(columnValues);
                foreach (var arr in joinedValues)
                {
                    file.WriteLine(string.Join(",", arr));
                }
            }
        }

        private static ISearchResponse<object> Search(Config _conf, ElasticClient client)
        {
            var searchResponse = client.Search<object>(q => q
                .QueryRaw(_conf.Query)
               .Source(f => f.Exclude(_conf.ExcludeFields.ToArray())
                .Include(_conf.IncludeFields.Count() != 0 ? _conf.IncludeFields : new string[] { })
                    )
                .Index(_conf.TenantID).Type(!string.IsNullOrEmpty(_conf.IndexType) ? _conf.IndexType: null)
                );
            return searchResponse;
        }
       }
}
