using Npgsql;

namespace RefugeAnimaux.Dal;

public static class Database
{
    // À adapter selon ta base PostgreSQL créée dans pgAdmin.
    public static string ConnectionString =
        "Host=localhost;Port=5432;Database=refuge_animaux;Username=postgres;Password=1234";

    public static NpgsqlConnection GetConnection()
    {
        var cnx = new NpgsqlConnection(ConnectionString);
        cnx.Open();
        return cnx;
    }
}
