using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEXIS.ASM.Services
{
    public interface ISummary : IDisposable
    {
        Task<string> get_summary(string dataset, string username);
        Task<string> get_analysisAsync(string dataset, string username, Boolean semantic_flag);
        Task<string> get_sampling_summary(string dataset, string username);
        Task<string> export_training_summary();
        void Dispose();
    }
}
