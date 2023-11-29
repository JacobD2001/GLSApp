using GLSApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GLSApp.Data.Enums;

namespace GLSApp.Interfaces
{
    public interface IGlsApiServiceInterface
    {
        Task<string> LoginAsync();
        Task<int?> PrepareBoxAsync(string session, Consign consign);
        Task<List<string>> GetLabelsAsync(string session, LabelMode mode);

    }
}
