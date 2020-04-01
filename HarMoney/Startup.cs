using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using EmailService;
using HarMoney.Contexts;
using HarMoney.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HarMoney
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            EmailConfiguration emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            emailConfig.Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();
            
            services.AddIdentity<User, AppRole>(opt =>
                {
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<IdentityAppContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = true;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequireLowercase = true;
                config.Password.RequireUppercase = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins(Environment.GetEnvironmentVariable("HARMONEY_FRONTEND"))
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddControllers();

            services.AddDbContext<IdentityAppContext>(cfg =>
                cfg.UseNpgsql(Environment.GetEnvironmentVariable("HARMONEY_CONNECTION")));

            //services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(opt =>
            //    opt.UseNpgsql(Environment.GetEnvironmentVariable("HARMONEY_CONNECTION")));
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Harmoney API", Version = "v1" });
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Harmoney API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);
            
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
