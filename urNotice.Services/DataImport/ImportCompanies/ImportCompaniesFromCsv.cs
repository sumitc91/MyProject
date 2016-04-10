using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Services.ImageService;
using urNotice.Services.Person;

namespace urNotice.Services.DataImport.ImportCompanies
{
    public class ImportCompaniesFromCsv:IImportCompanies
    {
        public bool ImportAllCompanies(string serverMapPath)
        {
            using (CsvReader reader = new CsvReader(@"C:\code\svn\final.csv"))
            {
                int flag = 0;
                foreach (string[] values in reader.RowEnumerator)
                {
                    if (values[0] == "Id") continue;
                    if (values[1] == "ISGN")
                    {
                        flag = 1;
                    }
                    if (flag == 0) continue;

                    string imageUrl = @values[3];
                    string saveLocation = serverMapPath + values[1].Replace(" ", "_") + "_image.png";
                    S3ImageUploadServices.SaveImageOnServer(imageUrl, saveLocation);
                    //S3ImageUploadServices.UploadSingleImageToS3FromPath(saveLocation,"company","png");

                    ImgurImageResponse img = S3ImageUploadServices.UploadSingleImageToS3FromPath(saveLocation, "OrbitPageCompanies", values[1]);

                    OrbitPageCompany newCompany = new OrbitPageCompany
                    {
                        CompanyName = values[1].Trim(),
                        averageRating = (float)Convert.ToDecimal(values[2]),
                        logoUrl = img.data.link_m,
                        squareLogoUrl = CommonConstants.CompanySquareLogoNotAvailableImage,
                        DisplayName = values[5].Trim(),
                        website = values[6],
                        workLifeBalanceRating = (float)Convert.ToDecimal(values[7]),
                        salaryRating = (float)Convert.ToDecimal(values[8]),
                        companyCultureRating = (float)Convert.ToDecimal(values[9]),
                        careerGrowthRating = (float)Convert.ToDecimal(values[10]),
                        specialities = values[11],
                        founded = values[12],
                        founder = values[13],
                        turnover = values[14],
                        headquarter = values[15],
                        employees = values[16],
                        competitors = values[17],
                        description = values[18]

                    };

                    IPerson adminModel = new Admin();
                    adminModel.CreateNewCompany(newCompany, "orbitpage@gmail.com");
                    Console.WriteLine("Row {0} has {1} values.", reader.RowIndex, values.Length);
                }
            }

            return true;
        }
    }
}
