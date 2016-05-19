using System.IO;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Services.Person;

namespace urNotice.Services.DataImport.ImportDesignations
{
    public class ImportDesinationsFromCsv : IImportDesignations
    {
        public bool ImportAllDesignations()
        {
            var reader = new StreamReader(System.IO.File.OpenRead(@"C:\code\svn\AllDesignations.csv"));
            
            while (!reader.EndOfStream)
            {
                var designationName = reader.ReadLine();
                IPerson adminModel = new Admin();

                if (designationName != null)
                {
                    designationName = designationName.Replace("&","And");
                    adminModel.CreateNewDesignation(designationName, "orbitpage@gmail.com");
                }
            }

            return true;
        }
    }
}
