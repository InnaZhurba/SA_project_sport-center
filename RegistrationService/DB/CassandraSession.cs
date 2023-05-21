using Cassandra;
using ISession = Cassandra.ISession;

namespace RegistrationService.DB
{
    public class CassandraSession
    {
        private static readonly CassandraSession _instance = new CassandraSession();
        private readonly ISession _session;

        private CassandraSession()
        {
            // Set up Cassandra cluster and session
            var cluster = Cluster.Builder()
                .AddContactPoint("127.0.0.1") 
                .WithDefaultKeyspace("gym_people_management") 
                .Build();

            _session = cluster.Connect();
        }

        public static CassandraSession Instance => _instance;

        public ISession GetSession()
        {
            return _session;
        }
    }
}