using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Services.Person;

namespace urNotice.Services.DataImport.ImportCompanyDesignationSalaries
{
    public class ImportCompanyDesignationSalariesFromCsv:IImportCompanyDesignationSalaries
    {
        public bool ImportCompanyDesignationAllSalaries()
        {
            var reader = new StreamReader(System.IO.File.OpenRead(@"C:\code\svn\salary.csv"));

            while (!reader.EndOfStream)
            {
                var readLine = reader.ReadLine();
                if (readLine != null)
                {
                    string[] line = readLine.Split(',');
                    IPerson adminModel = new Admin();
                    adminModel.CreateNewCompanyDesignationSalary(line[1], line[0], line[2], "orbitpage@gmail.com");
                }
            }

            return true;
        }
    }
}
