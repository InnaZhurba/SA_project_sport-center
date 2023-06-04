using Cassandra;
using ISession = Cassandra.ISession;

namespace MembershipManagementMicroservice.Infrastructure;

public class CassandraConfig
{
    private readonly ISession _session;

    public CassandraConfig()
    {
        var cluster = Cluster.Builder()
            .AddContactPoint("127.0.0.1")
            .WithDefaultKeyspace("membership_management")
            .Build();

        _session = cluster.Connect();
    }

    public void CreateKeyspace(string keyspace)
    {
        _session.Execute($"CREATE KEYSPACE IF NOT EXISTS {keyspace} WITH REPLICATION = {{ 'class' : 'SimpleStrategy', 'replication_factor' : 1 }}");
    }

    public void CreateTable(string tableName, string schema)
    {
        _session.Execute($"CREATE TABLE IF NOT EXISTS {tableName} ({schema})");
    }

    public void ExecuteQuery(string query)
    {
        _session.Execute(query);
    }

    public async Task<RowSet> ExecuteQuery(string query, params object[] args)
    {
        var statement = new SimpleStatement(query, args);
        return await _session.ExecuteAsync(statement).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}