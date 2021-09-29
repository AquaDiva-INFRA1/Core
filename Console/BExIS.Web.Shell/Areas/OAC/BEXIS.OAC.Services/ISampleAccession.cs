using BEXIS.OAC.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BExIS.OAC.Services
{
    public interface ISampleAccession : IDisposable
    {
        JObject fetchStudy(string studyID, DataSource url);
        JObject AddProjectsdataset(Dictionary<string, string> xx, string username, string metadata);
    }
}
