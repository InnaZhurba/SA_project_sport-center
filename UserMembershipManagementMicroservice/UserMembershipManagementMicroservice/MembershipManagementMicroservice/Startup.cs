using Cassandra;
using Confluent.Kafka;
using MembershipManagementMicroservice.Infrastructure;
using MembershipManagementMicroservice.Kafka;
using MembershipManagementMicroservice.Repository;
using MembershipManagementMicroservice.Repository.Interfaces;
using MembershipManagementMicroservice.Services;
using MembershipManagementMicroservice.Services.Interfaces;

namespace MembershipManagementMicroservice
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register Cassandra.ISession with its implementation
            services.AddSingleton<Cassandra.ISession>(provider =>
            {
                var cassandraCluster = Cluster.Builder()
                    .AddContactPoints("127.0.0.1")
                    .Build();

                var cassandraSession = cassandraCluster.Connect("membership_management");
                return cassandraSession;
            });

            // Configure Cassandra
            var cassandraConfig = new CassandraConfig();
            _configuration.Bind("Cassandra", cassandraConfig);
            services.AddSingleton(cassandraConfig);

            // Configure Kafka
            var kafkaConfig = new ProducerConfig()
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                ClientId = _configuration["Kafka:ClientId"]
            };
            _configuration.Bind("Kafka:Producer", kafkaConfig);
            services.AddSingleton(kafkaConfig);

            // Configure Kafka consumer
            var kafkaConsumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                ClientId = _configuration["Kafka:ClientId"],
                GroupId = _configuration["Kafka:GroupId"]
            };
            _configuration.Bind("Kafka:Consumer", kafkaConsumerConfig);
            services.AddSingleton(kafkaConsumerConfig);

            // Register repositories
            services.AddScoped<IRegistrationRepository, RegistrationRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IMembershipTypesRepository, MembershipTypesRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();

            // Register services
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IMembershipTypesService, MembershipTypesService>();
            services.AddScoped<IDiscountService, DiscountService>();
            
            services.AddSingleton<IKafkaProducerService, KafkaProducer>();
            services.AddSingleton<IKafkaConsumerService, KafkaConsumer>();

            // Add controllers
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IKafkaProducerService kafkaProducerService, IKafkaConsumerService kafkaConsumerService)
        {
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