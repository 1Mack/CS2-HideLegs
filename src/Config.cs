using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace HideLegs;

public partial class HideLegs
{
  public required HideLegsConfig Config { get; set; }
  public void OnConfigParsed(HideLegsConfig config)
  {
    if (config.Version != ConfigVersion) throw new Exception($"You have a wrong config version. Delete it and restart the server to get the right version ({ConfigVersion})!");

    if (config.Database.Host.Length < 1 || config.Database.Name.Length < 1 || config.Database.User.Length < 1)
    {
      throw new Exception($"You need to setup Database credentials in config!");
    }

    Config = config;
  }
}

public class HideLegsConfig : BasePluginConfig
{
  public override int Version { get; set; } = 2;
  [JsonPropertyName("Enabled")]
  public bool Enabled { get; set; } = true;
  [JsonPropertyName("UsePrivateFeature")]
  public bool UsePrivateFeature { get; set; } = false;

  [JsonPropertyName("Command")]
  public CommandC Command { get; set; } = new();
  public class CommandC
  {
    [JsonPropertyName("Prefix")]
    public string[] Prefix { get; set; } = ["legs", "hidelegs"];
    [JsonPropertyName("Permission")]
    public string[] Permission { get; set; } = [];
  }

  [JsonPropertyName("Database")]
  public DatabaseC Database { get; set; } = new();
  public class DatabaseC
  {
    [JsonPropertyName("Host")]
    public string Host { get; set; } = "";
    [JsonPropertyName("Port")]
    public int Port { get; set; } = 3306;
    [JsonPropertyName("User")]
    public string User { get; set; } = "";
    [JsonPropertyName("Password")]
    public string Password { get; set; } = "";
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "";
  }

}