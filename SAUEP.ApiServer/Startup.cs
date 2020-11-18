using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SAUEP.ApiServer.Configs;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Services;
using SAUEP.ApiServer.Connections;
using SAUEP.ApiServer.Repositories;

namespace SAUEP.ApiServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.ISSUER,
                            ValidateAudience = true,
                            ValidAudience = AuthOptions.AUDIENCE,
                            ValidateLifetime = true,
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            ValidateIssuerSigningKey = true,
                        };
                    });
            services.AddControllersWithViews();
            services.AddSingleton<IConnection, DataBaseConnection>();
            services.AddSingleton<IParser, JSONParser>();
            services.AddSingleton<IReader, FileReader>();
            services.AddSingleton<IGuardian, Guardian>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IAuthorization, AuthorizationService>();
            services.AddSingleton<IRegistration, RegistrationService>();
            services.AddSingleton<UserRepository, UserRepository>();
            services.AddSingleton<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddSingleton<DeviceRepository, DeviceRepository>();
            services.AddSingleton<PollRepository, PollRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
