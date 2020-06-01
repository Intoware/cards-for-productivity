using System;
using CardsForProductivity.API.Factories;
using CardsForProductivity.API.Hubs;
using CardsForProductivity.API.Models.Options;
using CardsForProductivity.API.Providers;
using CardsForProductivity.API.Repositories;
using Hangfire;
using Hangfire.Console;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace CardsForProductivity.API
{
    public class Startup
    {
        const string DefaultPolicyName = "default";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionStringsSection = Configuration.GetSection("ConnectionStrings");
            var storageSection = Configuration.GetSection("Storage");

            services.AddControllers();

            services.Configure<ConnectionStrings>(connectionStringsSection);
            services.Configure<StorageOptions>(storageSection);

            services.AddTransient<IRepositoryFactory, RepositoryFactory>();
            services.AddTransient<IDbContext, DbContext>();
            services.AddTransient<ISessionProvider, SessionProvider>();
            services.AddTransient<ISessionRepo, SessionRepo>();
            services.AddTransient<IStoryRepo, StoryRepo>();
            services.AddTransient<IUserRepo, UserRepo>();

            services.AddHangfire(options =>
            {
                var mongoConnectionString = connectionStringsSection.GetValue<string>("MongoDB");
                var mongoDatabaseName = storageSection.GetValue<string>("DatabaseName");

                options.UseConsole();
                var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
                options.UseMongoStorage(mongoClientSettings, mongoDatabaseName, new MongoStorageOptions
                {
                    Prefix = "Sessions:Hangfire",
                    MigrationOptions = new MongoMigrationOptions(MongoMigrationStrategy.Migrate)
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultPolicyName, policy =>
                {
                    // Workaround for allowing any origin with AllowCredentials
                    policy.WithOrigins(Configuration.GetValue<string>("Domain"))
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(60))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseCors(DefaultPolicyName);

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SessionHub>($"/{nameof(SessionHub)}");
            });
        }
    }
}
