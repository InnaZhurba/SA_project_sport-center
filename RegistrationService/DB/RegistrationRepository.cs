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
            //_logger.LogInformation($"CQL Query: {cql}");
            
            // convert DataTime birthdate to datetime for cassandra db
            var birthDate = new LocalDate(registration.BirthDate.Year, registration.BirthDate.Month, registration.BirthDate.Day);

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
        
        // GetAllRegistrations
        public async Task<List<Registration>> GetAllRegistrations()
        {
            var cql = @"SELECT * FROM gym_people_management.personal_info";
            var statement = new SimpleStatement(cql);
            var registrations = new List<Registration>();

            try
            {
                var result = await _session.ExecuteAsync(statement);
                foreach (var row in result)
                {
                    int day = row.GetValue<LocalDate>("birth_date").Day;
                    int month = row.GetValue<LocalDate>("birth_date").Month;
                    int year = row.GetValue<LocalDate>("birth_date").Year;
                    
                    DateTime dateTime = new DateTime(year, month, day);
                    
                    var registration = new Registration
                    {
                        //Id = row.GetValue<Guid>("id").ToString(),
                        FirstName = row.GetValue<string>("first_name"),
                        LastName = row.GetValue<string>("last_name"),
                        Email = row.GetValue<string>("email"),
                        Address = row.GetValue<string>("address"),
                        BirthDate = dateTime,
                        BodyFat = row.GetValue<float>("body_fat").ToString(CultureInfo.InvariantCulture),
                        Height = row.GetValue<float>("height").ToString(CultureInfo.InvariantCulture),
                        Weight = row.GetValue<float>("weight").ToString(CultureInfo.InvariantCulture)
                    };
                    registrations.Add(registration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting registrations from DB: {ex.Message}");
                throw;
            }

            return registrations;
        }
    }
}