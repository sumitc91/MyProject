using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Services.AdminService;

namespace ConsoleApplication1.Service
{
    public class FetchDataFromCsv
    {
        private string adminEmail = "sumitchourasia91@gmail.com";
        private static string accessKey = "AKIAIIUOBG6TQFINXYSQ";
        private static string secretKey = "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";
        //private static string authKey = "cb607bec-83d8-404f-8bb4-dc246e68be60";

        public string CsvReaderForAllDesignation()
        {
            var reader = new StreamReader(System.IO.File.OpenRead(@"C:\code\svn\final.csv"));
            //var list = new List<Location>();

            //while (!reader.EndOfStream)
            //{
            //    var line = reader.ReadLine();
            //    var values = line.Split(',');
            //    if (values[0].Equals("Id"))
            //        continue;

            //    String formatted_address = "";
            //    if (values[14] != null || values[14] != "")
            //    {
            //        formatted_address = HttpUtility.HtmlDecode(values[14].Replace(";;", ","));
            //    }

            //}

            
            return "test";
        }

        
    }
}
