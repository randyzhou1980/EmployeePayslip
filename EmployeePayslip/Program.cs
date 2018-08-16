using DataContextService;
using DataContextService.FileContext;
using LoggerService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayslipService;

namespace EmployeePayslip
{
    class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<StartUp>().GenerateEmployeePayslip();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // build configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            serviceCollection.AddOptions();

            serviceCollection.Configure<FileContextSetting>(config.GetSection("dataSettings:file"));

            // add services
            serviceCollection.AddTransient<IPayslipService, MonthlyPayslipService>();
            serviceCollection.AddTransient<IDataContextService, FileContextService>();
            serviceCollection.AddTransient<ILogService, ConsoleService>();

            // add app
            serviceCollection.AddSingleton<StartUp>();
        }
    }
}
