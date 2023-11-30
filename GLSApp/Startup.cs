using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using GLSApp.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using GLSApp.Interfaces;
using GLSApp.Services;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GLSApp.Repositories;

[assembly: FunctionsStartup(typeof(GLSApp.Startup))]

namespace GLSApp
{
    public class Startup : FunctionsStartup
    {
        //setting up DI and connection to database
        public override void Configure(IFunctionsHostBuilder builder)
        {        
            builder.Services.AddScoped<IConsignRepository, ConsignRepository>();
            builder.Services.AddScoped<IGlsApiServiceInterface, GlsApiService>();
            builder.Services.AddScoped<IPrinterService, PrinterService>();


            string connectionString = Environment.GetEnvironmentVariable("GLSAppConnectionString");
            builder.Services.AddDbContext<GLSContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}
