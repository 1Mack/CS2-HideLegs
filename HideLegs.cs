using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Admin;
using Microsoft.Extensions.Logging;
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
  Dictionary<ulong, bool> playersToShowMessage = [];


  //Thanks to https://github.com/dran1x/CS2-HideLowerBody
  public override void Load(bool hotReload)
  {
    if (!Config.Enabled)
    {
      Logger.LogWarning("This plugin is disabled");
      return;
    }

    RegisterListener<OnClientAuthorized>((playerSlot, steamid) =>
    {
      CCSPlayerController? player = Utilities.GetPlayerFromSlot(playerSlot);

      if (player == null || !player.IsValid || player.IsBot) return;

      if (Config.Command.Permission.Length > 0 && !AdminManager.PlayerHasPermissions(player, Config.Command.Permission))
        return;

      var steam = player.SteamID;

      Task.Run(() => GetClientInfo(player));
    });


    RegisterListener<OnClientDisconnect>(playerSlot =>
    {
      CCSPlayerController? player = Utilities.GetPlayerFromSlot(playerSlot);

      if (player == null || player.IsBot) return;

      if (!players.TryGetValue(player.SteamID, out bool value)) return;

      Task.Run(() => ExecuteAsync(@$"INSERT INTO {Config.Database.Prefix} (steamid, is_active) VALUES (@steamid, @isActive)
      ON DUPLICATE KEY UPDATE steamid = @steamid, is_active = @isActive", new { steamid = player.SteamID, isActive = value == true ? 1 : 0 }));
    });

    RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
    {
      if (@event.Userid == null || @event.Userid.IsBot) return HookResult.Continue;
      if (playersToShowMessage.TryGetValue(@event.Userid.SteamID, out bool hideLegs))
      {
        @event.Userid.PrintToChat($"{Localizer["Prefix"]} {Localizer[hideLegs ? "InformationsLoadedHidden" : "InformationsLoadedVisible"]}");
        playersToShowMessage.Remove(@event.Userid.SteamID);
      }
      return HookResult.Continue;
    });


    foreach (string command in Config.Command.Prefix)
    {
      AddCommand(command, "Hides the lower body view model of a player.", (player, command) =>
      {
        if (player == null || !player.IsValid) return;

        if (Config.Command.Permission.Length > 0 && !AdminManager.PlayerHasPermissions(player, Config.Command.Permission))
        {
          command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer["MissingCommandPermission"]}");
          return;
        }

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

        command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer[hideLegs ? "LegsAreNowHidden" : "LegsAreNowVisible"]}");
      });
    }


    Task.Run(CreateDatabaseTables);
    CheckVersion();
  }
}
