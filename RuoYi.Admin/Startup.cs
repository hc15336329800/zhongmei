using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using RuoYi.Admin.Authorization;
using RuoYi.Common.Files;
using RuoYi.Framework.Cache;
using RuoYi.Framework.Filters;
using RuoYi.Framework.RateLimit;

namespace RuoYi.Admin
{
    [AppStartup(10000)]
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsoleFormatter();
            services.AddCorsAccessor();
            services.AddRyJwt();
            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(GlobalExceptionFilter));
            });
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }).AddInject(options =>
            {
                options.ConfigureDataValidation(options =>
                {
                    options.GlobalEnabled = false;
                });
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var msg = string.Join(";", context.ModelState.Select(x => x.Value.Errors?.FirstOrDefault().ErrorMessage).ToList());
                    return new JsonResult(AjaxResult.Error(msg));
                };
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
#region 日志
            services.AddMonitorLogging();
            Array.ForEach(new[] { LogLevel.Information, LogLevel.Warning, LogLevel.Error }, logLevel =>
            {
                services.AddFileLogging("logs/application-{1}-{0:yyyy}-{0:MM}-{0:dd}.log", options =>
                {
                    options.FileNameRule = fileName => string.Format(fileName, DateTime.UtcNow, logLevel.ToString());
                    options.WriteFilter = logMsg => logMsg.LogLevel == logLevel;
                });
            });
#endregion
            services.AddRemoteRequest();
            services.AddSqlSugarScope();
            services.AddCache();
            services.AddSignalR();
            services.AddLazyCaptcha();
            services.ConfigureDynamicProxy();
            services.AddConcurrencyLimiter();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRyStaticFiles(env);
            app.UseRouting();
            app.UseCorsAccessor();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseInject();
            app.UseRateLimiter();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}