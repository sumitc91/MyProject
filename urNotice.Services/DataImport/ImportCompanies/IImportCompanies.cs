using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Services.DataImport.ImportCompanies
{
    public interface IImportCompanies
    {
        bool ImportAllCompanies(string saveLocation);
    }
}
