using Cassandra;
using ISession = Cassandra.ISession;

namespace MembershipManagementMicroservice.Infrastructure;


/// <summary>
///   This class is used to configure the Cassandra database.
///  It is used to create the keyspace and tables.
///  It is also used to execute queries.
/// </summary>
///  <remarks>
///  Sample code:
///  var cassandraConfig = new CassandraConfig();
///  cassandraConfig.CreateKeyspace("membership_management");
///  cassandraConfig.CreateTable("users", "id uuid PRIMARY KEY, username text, password text, email text");
///  cassandraConfig.CreateTable("memberships", "id uuid PRIMARY KEY, membership_type text, is_active text, start_date date, end_date date, user_id uuid");
///  </remarks>
public class CassandraConfig
{
    private readonly ISession _session;

    /// <summary>
    ///  The constructor creates a Cassandra cluster and session
    /// </summary>
    public CassandraConfig()
    {
        var cluster = Cluster.Builder()
            .AddContactPoint("127.0.0.1")
            .WithDefaultKeyspace("membership_management")
            .Build();

        _session = cluster.Connect();
    }

    /// <summary>
    ///     The CreateKeyspace method accepts a string argument and creates a keyspace if it does not exist
    /// </summary>
    /// <param name="keyspace">
    ///    The keyspace argument represents the name of the keyspace to be created in string format
    /// </param>
    ///  <remarks>
    ///  Sample code:
    ///  var cassandraConfig = new CassandraConfig();
    ///  cassandraConfig.CreateKeyspace("membership_management");
    ///  </remarks>
    public void CreateKeyspace(string keyspace)
    {
        _session.Execute($"CREATE KEYSPACE IF NOT EXISTS {keyspace} WITH REPLICATION = {{ 'class' : 'SimpleStrategy', 'replication_factor' : 1 }}");
    }

    /// <summary>
    ///   The CreateTable method accepts two string arguments and creates a table if it does not exist
    /// </summary>
    /// <param name="tableName">
    ///   The tableName argument represents the name of the table to be created in string format
    /// </param>
    /// <param name="schema">
    ///  The schema argument represents the schema of the table to be created in string format
    /// </param>
    ///  <remarks>
    ///  Sample code:
    ///  var cassandraConfig = new CassandraConfig();
    ///  cassandraConfig.CreateTable("users", "id uuid PRIMARY KEY, username text, password text, email text");
    ///  </remarks>
    public void CreateTable(string tableName, string schema)
    {
        _session.Execute($"CREATE TABLE IF NOT EXISTS {tableName} ({schema})");
    }

    /// <summary>
    ///     The ExecuteQuery method accepts a string argument and executes a query
    /// </summary>
    /// <param name="query">
    ///   The query argument represents the query to be executed in string format
    /// </param>
    ///  <remarks>
    ///  Sample code:
    ///  var cassandraConfig = new CassandraConfig();
    ///  cassandraConfig.ExecuteQuery("INSERT INTO users (id, username, password, email) VALUES (uuid(), 'testuser', 'testpassword', 'testemail')");
    ///  </remarks>
    public async Task<RowSet> ExecuteQuery(string query)
    {
       var statement = new SimpleStatement(query, null);
         return await _session.ExecuteAsync(statement).ConfigureAwait(false);
    }

    /// <summary>
    ///     The ExecuteQuery method accepts a string argument and executes a query
    /// </summary>
    /// <param name="query">
    ///  The query argument represents the query to be executed in string format
    /// </param>
    /// <param name="args">
    /// The args argument represents the arguments to be passed to the query
    /// </param>
    /// <returns>
    ///  Returns a RowSet object that contains the results of the query execution 
    /// </returns>
    ///  <remarks>
    ///  Sample code:
    ///  var cassandraConfig = new CassandraConfig();
    ///  var rowSet = cassandraConfig.ExecuteQuery("SELECT * FROM users WHERE username = ?", "testuser");
    ///  </remarks>
    public async Task<RowSet> ExecuteQuery(string query, params object[] args)
    {
        var statement = new SimpleStatement(query, args);
        return await _session.ExecuteAsync(statement).ConfigureAwait(false);
    }

    /// <summary>
    ///   The Dispose method is used to dispose of the Cassandra session object
    /// </summary>
    ///  <remarks>
    ///  Sample code:
    ///  var cassandraConfig = new CassandraConfig();
    ///  cassandraConfig.Dispose();
    ///  </remarks>
    public void Dispose()
    {
        _session.Dispose();
    }
}