using Cassandra;
using RegistrationService.DB;
using RegistrationService.Messaging;

namespace RegistrationService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Additional service configurations based on your microservice's needs

            services.AddControllers();

            services.AddTransient<IRegistrationService, Services.RegistrationService>();
            services.AddTransient<IRegistrationRepository, RegistrationRepository>();
            services.AddSingleton<IMessageProducer, KafkaMessageProducer>();

            // Register Cassandra.ISession with its implementation
            services.AddSingleton<Cassandra.ISession>(provider =>
            {
                // Perform the necessary setup and initialization to create an instance of Cassandra.ISession
               
                var cassandraCluster = Cluster.Builder()
                    .AddContactPoints("127.0.0.1")
                    .Build();

                var cassandraSession = cassandraCluster.Connect("gym_people_management");
                return cassandraSession;
            });

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.AddDebug();
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Additional app configuration

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}