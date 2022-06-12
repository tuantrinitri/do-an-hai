using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using CMS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace CMS
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            //CreateHostBuilder(args).Build().Run();

            // log hệ thống mặc định của microsoft
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()

                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                // tạo dữ liệu tới db dulich
                var seed = args.Contains("FirstDb");
                if (seed)
                {
                    args = args.Except(new[] { "FirstDb" }).ToArray();
                }
                // tạo host => khai báo với hệ thống chương trình sẽ chạy 
                var host = CreateHostBuilder(args).Build();
                // tạo dữ liệu ảo từ các file json lấy từ /Data/SeeData.cs
                if (seed)
                {
                    Log.Information("Seeding database...");
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString("DefaultConnection");
                    SeedData.EnsureSeedData(connectionString);
                    Log.Information("Done seeding database.");
                    return 0;
                }

                Log.Information("Starting host...");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Biến môi trường khởi tạo cho chương trình. Lấy biến môi trường từ appsettings.json
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// 

        /*
        * Function khai báo với hthong chương trình sẽ lấy port 5000 làm port chạy chương trình
        */
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseWindowsService()
                //.UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var isDevelopment = environment == Environments.Development;
                    webBuilder.UseUrls("https://localhost:5000");
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>(); //DI
                    
                });
    }
}

//if (isDevelopment)
//{
//  webBuilder.UseUrls("http://*:5000");
//}
//else
//{
//  webBuilder.UseUrls("http://*:2020");
//}
