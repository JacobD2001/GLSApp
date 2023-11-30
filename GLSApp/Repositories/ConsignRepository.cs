using GLSApp.Data;
using GLSApp.Interfaces;
using GLSApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Repositories
{
    public class ConsignRepository : IConsignRepository
    {
        private readonly GLSContext _context;

        public ConsignRepository(GLSContext context)
        {
            _context = context;
        }

        public async Task<int?> AddAsync(Consign consign)
        {
            _context.Consigns.Add(consign);
            await _context.SaveChangesAsync();
            return consign.Id;
        }

        public async Task<List<Consign>> GetConsignmentsAsync()
        {
            return await _context.Consigns.ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        // Return list of labels from all consignments
        public async Task<List<string>> GetAllLabelsAsync()
        {
            List<Consign> consignments = await GetConsignmentsAsync();

            return consignments.SelectMany(c => c.Labels).ToList();

        }      

    }
}
