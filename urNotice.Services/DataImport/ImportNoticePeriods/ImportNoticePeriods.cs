using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Services.Person;

namespace urNotice.Services.DataImport.ImportNoticePeriods
{
    public class ImportNoticePeriods : IImportNoticePeriods
    {
        public bool ImportCompanyAllDesignationNoticePeriods()
        {
            var reader = new StreamReader(System.IO.File.OpenRead(@"C:\code\svn\OrbitPage\salary.csv"));
            IPerson adminModel = new Admin();
            double salary = 0.0;

            while (!reader.EndOfStream)
            {
                var readLine = reader.ReadLine();
                if (readLine != null)
                {
                    string[] line = readLine.Split(',');                    
                    
                    try
                    {                        
                        salary = Convert.ToDouble(line[2]);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    String noticePeriodRange = string.Empty;
                    if (salary < 1.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.ZeroToFifteen.ToString();
                    }
                    else if (salary < 2.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.FifteenToThirty.ToString();
                    }
                    else if (salary < 3.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.FifteenToThirty.ToString();
                    }
                    else if (salary < 6.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.ThirtyToFourtyFive.ToString();
                    }
                    else if (salary < 10.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.FourtyfiveToSixty.ToString();
                    }
                    else if (salary < 13.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.SixtytoSeventyfive.ToString();
                    }
                    else if (salary < 20.0)
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.SixtytoSeventyfive.ToString();
                    }
                    else
                    {
                        noticePeriodRange = NoticePeriodRangeEnum.SeventyfiveToNinety.ToString();
                    }

                    adminModel.CreateNewCompanyDesignationNoticePeriod(line[1], line[0], noticePeriodRange, "orbitpage@gmail.com");
                }
            }

            return true;
        }
    }
}
