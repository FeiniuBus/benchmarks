using System.Net.Http;
using FeiniuBus.MongoDB;
using HttpClientTestCore.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace HttpClientTestCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            var httpClient = new HttpClient();
            services.AddSingleton(httpClient);

            services.AddSingleton(Configuration);
            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = Configuration.GetConnectionString("RedisConnectionString");
            //});

            services.AddDbContext<EfDbContext>(
                options =>
                {
                    options.UseMySQL(Configuration.GetConnectionString("MySqlDbConnectionString"));
                });

            services.AddScoped<IMongoDbContext, DbContext.MongoDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}