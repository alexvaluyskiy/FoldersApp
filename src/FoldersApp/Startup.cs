using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FoldersApp.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using FoldersApp.Core.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using FoldersApp.Services;

namespace FoldersApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(c =>
            {
                c.Filters.Add(typeof(MyExceptionFilter));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "FileSystem API", Version = "v1" });
            });

            var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb6;Trusted_Connection=True;";
            services.AddDbContext<FoldersContext>(options => {
                options.UseSqlServer(connection);
            });

            services.AddTransient<IFoldersRepository, FoldersRepository>();
            services.AddTransient<FoldersService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileSystem API V1");
            });
        }
    }
}
