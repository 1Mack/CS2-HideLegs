using System.Drawing;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;

namespace HideLegs;

public partial class HideLegs
{
  public async void CreateDatabaseTables()
  {
    BuildDatabaseConnectionString();
    try
    {
      await ExecuteAsync(@$"CREATE TABLE IF NOT EXISTS `{Config.Database.Prefix}` 
          (`id` INT NOT NULL AUTO_INCREMENT, `steamid` varchar(64) NOT NULL UNIQUE, is_active TINYINT(1) DEFAULT 0, PRIMARY KEY (`id`)) 
          ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci");

    }
    catch (Exception)
    {
      throw new Exception($"{Localizer["Prefix"]} Unable to create tables!");
    }
  }
  public async void GetClientInfo(CCSPlayerController player)
  {
    ulong steamid = player.SteamID;

    var result = await QueryAsync($"SELECT * FROM `{Config.Database.Prefix}` WHERE `steamid` = @steamid", new { steamid = steamid.ToString() });


    if (result.Count == 0) return;

    bool value = result[0].is_active;

    if (!players.TryAdd(player.SteamID, value))
      players[player.SteamID] = value;

    if (!playersToShowMessage.TryAdd(player.SteamID, value))
      playersToShowMessage[player.SteamID] = value;

    Server.NextFrame(() =>
    {
      AddTimer(3.5f, () =>
      {
        SetLegs(steamid, value);

      });
    });

  }
  public static void SetLegs(ulong steamid, bool showLegs)
  {

    var player = Utilities.GetPlayerFromSteamId(steamid);

    if (player == null || !player.IsValid || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null) return;

    player.PlayerPawn.Value.Render = Color.FromArgb(showLegs ? 254 : 255,
                      player.PlayerPawn.Value.Render.R, player.PlayerPawn.Value.Render.G, player.PlayerPawn.Value.Render.B);

    Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseModelEntity", "m_clrRender");
  }

  internal class IRemoteVersion
  {
    public required string tag_name { get; set; }
  }
  public void CheckVersion()
  {
    Task.Run(async () =>
    {
      using HttpClient client = new();
      try
      {
        client.DefaultRequestHeaders.UserAgent.ParseAdd("HideLegs");
        HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/1Mack/CS2-HideLags/releases/latest");

        if (response.IsSuccessStatusCode)
        {
          IRemoteVersion? toJson = JsonSerializer.Deserialize<IRemoteVersion>(await response.Content.ReadAsStringAsync());

          if (toJson == null)
          {
            Logger.LogWarning("Failed to check version1");
          }
          else
          {
            int comparisonResult = string.Compare(ModuleVersion, toJson.tag_name[1..]);

            if (comparisonResult < 0)
            {
              Logger.LogWarning("Plugin is outdated! Check https://github.com/1Mack/CS2-HideLags/releases/latest");
            }
            else if (comparisonResult > 0)
            {
              Logger.LogInformation("Probably dev version detected");
            }
            else
            {
              Logger.LogInformation("Plugin is up to date");
            }
          }

        }
        else
        {
          Logger.LogWarning("Failed to check version2");
        }
      }
      catch (HttpRequestException ex)
      {
        Logger.LogError(ex, "Failed to connect to the version server.");
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "An error occurred while checking version.");
      }
    });
  }
}
