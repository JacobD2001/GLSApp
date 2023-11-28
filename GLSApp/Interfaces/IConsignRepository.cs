using GLSApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Interfaces
{
    public interface IConsignRepository
    {
        Task<List<Consign>> GetConsignmentsAsync();
        bool Save();
        Task<int?> AddAsync(Consign consign);


    }
}
