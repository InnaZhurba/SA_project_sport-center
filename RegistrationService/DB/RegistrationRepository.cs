using Cassandra;
using System;
using System.Globalization;
using System.Threading.Tasks;
using RegistrationService.Models;
using ISession = Cassandra.ISession;

namespace RegistrationService.DB
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ILogger<RegistrationRepository> _logger;
        private readonly ISession _session;

        public RegistrationRepository(ISession session, ILogger<RegistrationRepository> logger)
        {
            _session = session;
            _logger = logger;
        }

        public async Task SaveRegistration(Registration registration)
        {
            // Logic to save registration using personal data
            
            var cql = @"INSERT INTO gym_people_management.personal_info (
                                                id, first_name, last_name, email, address, birth_date,
                                                body_fat, height, weight)
                        VALUES ( ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            
            // convert string to int 16 bytes
            //var id = Guid.Parse(registration.Id);
            _logger.LogInformation($"CQL Query: {cql}");
            
            // convert DataTime birthdate to datetime for cassandra db
            var birthDate = new LocalDate(registration.BirthDate.Year, registration.BirthDate.Month, registration.BirthDate.Day);
            
            _logger.LogInformation($"{registration.BirthDate.Year}");
            _logger.LogInformation($"{birthDate}");
            
            var statement = new SimpleStatement(cql)
                .Bind(Guid.NewGuid(),
                    registration.FirstName,
                    registration.LastName,
                    registration.Email,
                    registration.Address,
                    birthDate,
                    float.Parse(registration.BodyFat, CultureInfo.InvariantCulture),
                    float.Parse(registration.Height, CultureInfo.InvariantCulture),
                    float.Parse(registration.Weight, CultureInfo.InvariantCulture)
                    
                    );
                
            //var boundValues = statement.QueryValues;
            
            _logger.LogInformation($"CQL Query: {cql}");

            try
            {
                await _session.ExecuteAsync(statement);
                _logger.LogInformation("Registration saved successfully in DB.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving registration in DB: {ex.Message}");
                throw;
            }
        }
    }
}