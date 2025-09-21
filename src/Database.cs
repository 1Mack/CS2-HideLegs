using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace HideLegs;

public partial class HideLegs
{
  private string _databaseConnectionString = string.Empty;

  private void BuildDatabaseConnectionString()
  {
    var builder = new MySqlConnectionStringBuilder
    {
      Server = Config.Database.Host,
      UserID = Config.Database.User,
      Password = Config.Database.Password,
      Database = Config.Database.Name,
      Port = (uint)Config.Database.Port,
    };

    _databaseConnectionString = builder.ConnectionString;
  }
  public async Task<List<dynamic>> QueryAsync(string query, object? parameters = null)
  {
    try
    {
      using MySqlConnection connection = new(_databaseConnectionString);

      await connection.OpenAsync();

      var queryResult = await connection.QueryAsync(query, parameters);

      await connection.CloseAsync();

      return queryResult.ToList();
    }
    catch (Exception ex)
    {
      Logger.LogError(ex.Message);
      return [];
    }
  }
  public async Task ExecuteAsync(string query, object? parameters = null)
  {

    try
    {
      using MySqlConnection connection = new(_databaseConnectionString);

      await connection.OpenAsync();

      var queryResult = await connection.ExecuteAsync(query, parameters);

      await connection.CloseAsync();
    }
    catch (Exception ex)
    {
      Logger.LogError(ex.Message);
    }
  }

}
