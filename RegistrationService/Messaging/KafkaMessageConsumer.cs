using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RegistrationService.Models;

namespace RegistrationService.Messaging
{
    public class KafkaMessageConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaMessageConsumer> _logger;

        public KafkaMessageConsumer(IConfiguration configuration, ILogger<KafkaMessageConsumer> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_configuration["Kafka:Topic"]);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    _logger.LogInformation($"Received message: {consumeResult.Message.Value}");

                    // Add your message handling logic here
                    // For example, you can deserialize the message JSON and process the data

                    // Deserialize the message JSON
                    var registration = JsonConvert.DeserializeObject<Registration>(consumeResult.Message.Value);

                    // Perform your processing logic using the deserialized registration object
                    // For example:
                    await HandleRegistration(registration);

                    consumer.Commit(consumeResult);
                }
            }
            catch (OperationCanceledException)
            {
                // This exception is expected when the service is stopping.
            }
            finally
            {
                consumer.Close();
            }
        }

        private async Task HandleRegistration(Registration registration)
        {
            try
            {
                // Add your logic to handle the registration data
                // For example, you can perform some validation, save to a database, or trigger other actions

                _logger.LogInformation($"Handling registration: , " +
                                       $"Email={registration.Email}, " +
                                       $"Address={registration.Address}, " +
                                       $"LastName={registration.LastName}, " +
                                       $"FirstName={registration.FirstName}, " +
                                       $"BodyFat={registration.BodyFat}, " +
                                       $"Height={registration.Height}, " +
                                       $"Weight={registration.Weight}");
//Id={registration.Id}
                // Your processing logic goes here
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling the registration.");
                // You can handle the exception as needed, such as logging or rethrowing.
            }
        }
    }
}
