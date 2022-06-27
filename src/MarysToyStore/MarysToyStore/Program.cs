using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MarysToyStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("MarysToyStore is starting...");
                CreateHostBuilder(args).Build().Run(); // Run the web host.
            }
            catch (Exception e)
            {
                // The webserver crashed for some reason.
                Log.Fatal(e, "The program terminated unexpectedly.");
            }
            finally
            {
                // The server is shutting down, make sure that the log files are written to
                Log.Information("MarysToyStore is stopping...");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // Add this line.
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
