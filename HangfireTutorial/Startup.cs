namespace HangfireTutorial
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Hangfire.Mongo;
    using Hangfire;
    using Hangfire.Storage;
    using System;

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
            services.AddControllers();

            var options = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    BackupStrategy = MongoBackupStrategy.None,
                    Strategy = MongoMigrationStrategy.Drop
                }
            };

            services.AddHangfire(x => x.UseMongoStorage(Configuration.GetConnectionString("HangfireConnection"), options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard()
                .UseHangfireServer();

            // Removes all recurring jobs
            using (var connection = JobStorage.Current.GetConnection())
            {
                StorageConnectionExtensions.GetRecurringJobs(connection)
                    .ForEach(job => RecurringJob.RemoveIfExists(job.Id));
            }

            RecurringJob.AddOrUpdate<HangfireJobs>(nameof(HangfireJobs.GoodMorning), x => x.GoodMorning(), Cron.Daily(8));
        }

        public class HangfireJobs
        {
            public void GoodMorning()
            {
                Console.WriteLine("Good Morning!");
            }
        }
    }
}
