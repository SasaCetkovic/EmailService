using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Email.Api.Extensions;
using Email.Api.Services;
using Email.Shared.Data;
using System;

namespace Email.Api
{
	public class Startup
    {
		private readonly IConfiguration _configuration;
		private readonly IHostingEnvironment _env;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			_configuration = configuration;
			_env = env;
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			var builder = services.AddMvcCore();
			builder.AddAuthorization();
			builder.AddApiExplorer();
			builder.AddDataAnnotations();
			builder.AddFormatterMappings();
			builder.AddJsonFormatters();
			builder.AddJsonOptions(
				options =>
				{
					options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				});

			services.AddCors(SetupCors);
			services.ConfigureSwagger(_configuration);

			// Basic auth
			services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
				    .AddBasicAuthentication<AuthService>();

			services.AddDbContext<EmailDbContext>(opt => opt.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

			services.ConfigureCustomDependencies();
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
			if (_env.IsDevelopment())
			{
				var dbContext = serviceProvider.GetService<EmailDbContext>();
				dbContext.Database.EnsureCreated();
			}

			app.UseCustomExceptionHandler();
			app.UseSwagger();
			app.UseSwaggerUiCustomSettings(_configuration);

			app.UseCors("AllowAll");
			app.UseAuthentication();

			app.UseMvc();
		}


		private void SetupCors(CorsOptions options)
		{
			options.AddPolicy("AllowAll", policyBuilder =>
			{
				policyBuilder.AllowAnyOrigin()
							 .AllowAnyMethod()
							 .AllowAnyHeader();
			});
		}
	}
}
