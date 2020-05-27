using System;
using System.ServiceModel;
using AutoMapper;
using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.Services;
using CRUDWebService.DataLayer.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SoapCore;

namespace CRUDWebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddSoapCore();
            services.AddMvc(x => x.EnableEndpointRouting = false);

            services.AddDbContext<UniversityContext>(options =>
                options.UseNpgsql(GetConnectionString()));


            services.AddTransient<IUniversityService, UniversityService>();
            services.AddTransient<IStudentService, StudentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSoapEndpoint<IUniversityService>("/Service.svc", new BasicHttpBinding(), SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<IUniversityService>("/Service.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);

            app.UseMvc();
        }

        public string GetConnectionString()
        {
            var uriString = "postgres://jnlzccfb:Bsh9MYNQvP2pHyvfndjhJ3BXd090drVD@kandula.db.elephantsql.com:5432/jnlzccfb";
            var uri = new Uri(uriString);
            var db = uri.AbsolutePath.Trim('/');
            var user = uri.UserInfo.Split(':')[0];
            var passwd = uri.UserInfo.Split(':')[1];
            var port = uri.Port > 0 ? uri.Port : 5432;
            var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
                uri.Host, db, user, passwd, port);
            return connStr;
        }
    }
}
