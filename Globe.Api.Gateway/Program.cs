using Serilog;
using System.Reflection;
using System.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Globe.Shared.Helpers;
using Globe.Shared.Models;


namespace Globe.Api.Gateway
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static string CurrentPath { get; set; }

        public static bool RunAsService;


        public static int Main(string[] args)
        {
            CurrentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            System.IO.Directory.SetCurrentDirectory(CurrentPath);

            ILogger<Program> logger = null;

            var webHostArgs = args.Where(arg => arg != "--console").ToArray();

            // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
            // logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
            // set up successfully.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Starting up!");

            try
            {
                ConfigurationBuilder();

                RunAsService = !(Debugger.IsAttached || args.Contains("--console") ||
                    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                    Environment.GetEnvironmentVariable("RUN_ON_IIS") == "true");

                var webBuilder = CreateWebHostBuilder(webHostArgs).Build();
                IWebHostEnvironment env = (IWebHostEnvironment)webBuilder.Services.GetService(typeof(IWebHostEnvironment));
                logger = (ILogger<Program>)webBuilder.Services.GetService(typeof(ILogger<Program>));
                logger.LogInformation("RunAsService: {RunAsService}.", RunAsService);

                if (RunAsService)
                {
                    webBuilder.RunAsService();
                }
                else
                {
                    webBuilder.Run();
                }

                StopLogging(logger);
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during bootstrapping. Service {ServiceName} is Stopping now...", Configuration.GetSection("ServiceInformation").GetValue<string>("ServiceName"));
                if (logger != null)
                    logger.LogCritical(ex, "An unhandled exception occured during bootstrapping. Service {ServiceName} is Stopping now...", Configuration.GetSection("ServiceInformation").GetValue<string>("ServiceName"));
                return 1;
            }
            finally
            {
                logger = null;
                Log.CloseAndFlush();
            }
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            Log.Information("Starting web host builder creation...");
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, config) =>
                {
                    // Specifying the routes config to the app config. 
                    // This will be used by Ocelot
                    string environmentName = host.HostingEnvironment.EnvironmentName.ToLower();
                    config.AddJsonFile(Path.Combine(CurrentPath, $"appsettings.json"));

                    // Set current environment config file
                    var environmentConfigFile = Path.Combine(CurrentPath, environmentName.ToLower() == "development" ? "appsettings.Development.json" : "appsettings.Production.json");

                    // Check if environment config file exists or not, if exists, build configuration
                    if (File.Exists(environmentConfigFile)) config.AddJsonFile(environmentConfigFile);

                    config.AddJsonFile(Path.Combine(CurrentPath, "Config", $"gateway_config.{environmentName}.json"));
                    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
                        config.AddEnvironmentVariables();
                });

            //Get Urls from SystemSettings (ONLY IF Docker is NOT used)  
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
                builder.UseConfiguration(Configuration);

            builder.UseStartup<Startup>()
               .ConfigureKestrel(serverOptions =>
               {
                   Console.WriteLine("Configuring Kestrel...");
                   serverOptions.ConfigureHttpsDefaults(listenOptions => SetCertificate(listenOptions));
               })
               /*.UseSerilog((context, loggerConfiguration) =>
               {
                   // read logger's config and set logger...
                   LoggerHelper loggerHelper = new LoggerHelper();
                   loggerHelper.Configure(context, loggerConfiguration, Configuration);
               })*/
               .ConfigureServices((builder, services) =>
               {
                   var inj = new InjectProgramModel();
                   inj.RunAsService = RunAsService;
                   services.AddSingleton(inj);
               });
            Log.Information("Done creating web host builder!");
            return builder;
        }

        private static void SetCertificate(HttpsConnectionAdapterOptions listenOptions)
        {
            // certificate is an X509Certificate2
            if (Configuration["Certificate:Enabled"].ToLower() != "true") return;

            string outputCertificateFile = Configuration["Certificate:Path"];
            string outputCertificatePassword = EncryptionHelper.DecryptString(Configuration["Certificate:Password"]);

            Console.WriteLine("Loading Certificate...");
            if (!new FileInfo(outputCertificateFile).Exists)
            {
                Console.WriteLine($"Certificate {outputCertificateFile} not found");
                throw new Exception($"Certificate {outputCertificateFile} not found");
            }
            var tlsCertificate = new X509Certificate2(outputCertificateFile, outputCertificatePassword);
            listenOptions.ServerCertificate = tlsCertificate;
        }

        private static void StopLogging(ILogger<Program> logger)
        {
            EventId stopapp = new EventId(1002, "STOP_APP");
            logger.LogInformation(stopapp, " >>>>>>>> Service {ServiceName} is Stopping normally <<<<<<<<<", Configuration.GetSection("ServiceInformation").GetValue<string>("ServiceName"));
        }

        private static void ConfigurationBuilder()
        {
            try
            {
                var apps1 = new List<string>() { CurrentPath, "appsettings.json" };
                var appSettingsPath1 = Path.GetFullPath(Path.Combine(apps1.ToArray()));

                var apps2 = new List<string>()
                {
                    CurrentPath,
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json"
                };
                var appSettingsPath2 = Path.GetFullPath(Path.Combine(apps2.ToArray()));

                var builder = new ConfigurationBuilder();
                builder.SetBasePath(CurrentPath);
                builder.AddJsonFile(appSettingsPath1, optional: false, reloadOnChange: true)
                        .AddJsonFile(appSettingsPath2, optional: true, reloadOnChange: true);
                builder.AddEnvironmentVariables(); // <--- this allows Docker to change config by putting environmental variables

                Configuration = builder.Build();
            }
            catch (Exception ex)
            {
                Log.Fatal("Configuration build exception: {ex}", ex);
                throw;
            }
        }
    }

}
