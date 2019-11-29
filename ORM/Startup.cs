using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORM.Dapper;
using ORM.EfCore;
using ORM.LogProvider;

namespace ORM
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //public static readonly LoggerFactory AppLoggerFactory
        //    = new LoggerFactory(new[]
        //    {
        //        new ConsoleLoggerProvider((category, level)
        //            => category == DbLoggerCategory.Database.Command.Name
        //               && level == LogLevel.Information, true), 
        //    });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<SqlContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                //.UseLoggerFactory(AppLoggerFactory));
            services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAuthorRepository), typeof(AuthorRepository));
            services.AddScoped(typeof(IBookRepository), typeof(BookRepository));
            services.AddScoped(typeof(ITagRepository), typeof(TagRepository));
            services.AddScoped(typeof(IBookTagRepository), typeof(BookTagRepository));
            services.AddScoped(typeof(ILogger),typeof(DBLogger));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddContext(LogLevel.Information, Configuration.GetConnectionString("DefaultConnection"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=ef}/{action=Index}/{id?}");
            });
        }

    }
   
}
