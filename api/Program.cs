using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace api {
    /*
     *  dotnet ef dbcontext scaffold connexion_string Pomelo.EntityFrameworkCore.MySql -c ProAdvisorContext -o Model
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