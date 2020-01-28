using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api {
    /*
     *  dotnet ef dbcontext scaffold "Remplacer par la connection string" Pomelo.EntityFrameworkCore.MySql -c ProAdvisorContext -o Model
     *  dotnet aspnet-codegenerator controller -name EntrepriseController -async -api -m Entreprise -dc projetsyntheseContext -outDir Controllers
     */
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });
    }
}