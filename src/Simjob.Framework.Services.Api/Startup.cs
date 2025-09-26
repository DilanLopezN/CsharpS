using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.IoC;
using Simjob.Framework.Services.Api.Extensions;
using Simjob.Framework.Services.Api.Interfaces;
using Simjob.Framework.Services.Api.Middleware;
using Simjob.Framework.Services.Api.Services;
using Simjob.Framework.Services.Api.ViewModels;
using System;
using System.IO;
using System.Reflection;
using WorkTiming.Services.Api.Configurations;

namespace Simjob.Framework.Services.Api
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

            services.AddCors(opt => opt.AddPolicy("*", build => build.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            services.AddControllers()
                .AddNewtonsoftJson();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;

                options.MultipartBodyLengthLimit = long.MaxValue; // Define o tamanho máximo permitido para o corpo da requisição (multipart/form-data)
                options.MemoryBufferThreshold = int.MaxValue;
            });
            services.AddAuthenticationConfiguration(Configuration);

            services.AddAuthorizationConfiguration();
            //api-key
            services.AddScoped<IAuthorizationHandler, ApiKeyHandler>();
            services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
            services.AddScoped<ApiKeyAuthFilter>();
            //services.AddTransient<IMiddleware, YourMiddleware>();
            //services.AddStorageConfiguration(Configuration);

            services.AddSwaggerConfiguration();

            services.AddMemoryCache();

            services.AddScoped<ITokenService, TokenService>();
            NativeInjection.InjectDependecies(services);

            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });          

            services.Configure<FiskLpConfig>(options =>
            {
                Configuration.GetSection(nameof(FiskLpConfig)).Bind(options);
            });


            //string teste = JsonConvert.SerializeObject(new string[] { "1", "array" });
            //dynamic tested = JsonConvert.DeserializeObject<dynamic>(teste);
            //JArray jr = new JArray();
            //jr.FirstOrDefault(t => t.Value<string>() == "array");
            //var tye = tested.GetType();
            //var type = tested.Type;
            //var tb = type.ToString() == "Array";
            //bool multiType = tested.Type.ToString() == "Array";
            //var a = ((JArray)tested).FirstOrDefault(t => t.Value<string>() == "array");



            //bool isArray = multiType ? ((List<string>)tested).IndexOf("array") >= 0 :
            //                            tested.ToString() == "array";


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //string path = Path.Combine(env.WebRootPath, "images/");
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                try
                {
                    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = long.MaxValue;
                    await Middleware.ContextHandler.HandleIncomingRequests(context, next);
                }
                catch (Exception e)
                {
                    Program.mainStorage.GetSubStorage("error-logs").Set("dump-" + System.Text.RegularExpressions.Regex.Match(DateTime.Now.ToString(), @"\d+").Value, e.ToString());
                    await Middleware.ContextHandler.JSONResponse(context, new { success = false, data = new object[] { new { key = "Server Error", value = $"Server Error: {e.Message} {e.InnerException?.Message ?? ""}" } } }, StatusCodes.Status400BadRequest);
                }
            });



            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simjob Framework Api v1"));
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("*");
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
