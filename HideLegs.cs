using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using static CounterStrikeSharp.API.Core.Listeners;

namespace HideLegs;

[MinimumApiVersion(199)]
public partial class HideLegs : BasePlugin, IPluginConfig<HideLegsConfig>
{
  public override string ModuleName => "HideLegs";
  public override string ModuleAuthor => "1MaaaaaacK";
  public override string ModuleDescription => "Allows players to hide their first person legs model. (lower body view model)";
  public override string ModuleVersion => "1.0.0";
  public static int ConfigVersion => 1;
  Dictionary<ulong, bool> players = [];

  //Thanks to https://github.com/dran1x/CS2-HideLowerBody
  public override void Load(bool hotReload)
  {
    RegisterListener<OnClientAuthorized>((playerSlot, steamid) =>
    {
      CCSPlayerController? player = Utilities.GetPlayerFromSlot(playerSlot);

      if (player == null || !player.IsValid || player.IsBot) return;

      var steam = player.SteamID;

      Task.Run(() => GetClientInfo(player.SteamID));
    });
    RegisterListener<OnClientDisconnect>(playerSlot =>
    {
      CCSPlayerController? player = Utilities.GetPlayerFromSlot(playerSlot);

      if (player == null || player.IsBot) return;

      if (!players.ContainsKey(player.SteamID)) return;

      Task.Run(() => ExecuteAsync(@$"INSERT INTO {Config.Database.Prefix} (steamid, is_active) VALUES (@steamid, @isActive)
      ON DUPLICATE KEY UPDATE steamid = @steamid, is_active = @isActive", new { steamid = player.SteamID, isActive = players[player.SteamID] == true ? 1 : 0 }));
    });

    foreach (string command in Config.Command)
    {
      AddCommand(command, "Hides the lower body view model of a player.", (player, command) =>
      {
        if (player == null || !player.IsValid) return;

        bool hideLegs;

        if (players.TryGetValue(player.SteamID, out bool value))
        {
          hideLegs = !value;
          players[player.SteamID] = hideLegs;
        }
        else
        {
          hideLegs = false;
          players.Add(player.SteamID, hideLegs);
        }
        SetLegs(player.SteamID, hideLegs);

        command.ReplyToCommand(hideLegs ? "Legs are now hidden" : "Legs are now visible");
      });
    }


    Task.Run(CreateDatabaseTables);
  }
}
