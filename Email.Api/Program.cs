using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;

namespace Email.Api
{
	public class Program
	{
		private static IConfigurationRoot _config;

		public static IConfiguration Config { get; private set; }

		public static void Main(string[] args)
		{
			_config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("hosting.json", optional: true)
				.Build();

			var webHost = BuildWebHost(args);
			Config = webHost.Services.GetService<IConfiguration>();
			webHost.Run();
		}

		private static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(_config)
				.UseStartup<Startup>()
				.ConfigureLogging(ConfigureLogger)
				.UseNLog()
				.Build();
		}

		private static void ConfigureLogger(WebHostBuilderContext ctx, ILoggingBuilder logging)
		{
			logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
			logging.AddDebug();
		}
	}
}
