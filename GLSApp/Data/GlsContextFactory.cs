using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GLSApp.Data
{
    //class for ef to see dbcontext during migrations and so on
    public class GlsContextFactory : IDesignTimeDbContextFactory<GLSContext>
    {
        public GLSContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GLSContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("GLSAppConnectionString"));

            return new GLSContext(optionsBuilder.Options);
        }

    }
}
