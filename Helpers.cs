using System.Drawing;
using CounterStrikeSharp.API;

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
  public async void GetClientInfo(ulong steamid)
  {

    var result = await QueryAsync($"SELECT * FROM `{Config.Database.Prefix}` WHERE `steamid` = @steamid", new { steamid = steamid.ToString() });

    bool value = result.Count > 0 ? result[0].is_active : false;

    if (players.ContainsKey(steamid))
      players[steamid] = value;
    else
      players.Add(steamid, value);


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
}