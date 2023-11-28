using System.Collections.Generic;
using GLSApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GLSApp.Data
{
    public class GLSContext : DbContext
    {   
        public GLSContext(DbContextOptions<GLSContext> options) : base(options)
        {
        }
        
        public DbSet<Consign> Consigns { get; set; }

    }
}
