using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace WeaponsGiver {
  [MinimumApiVersion(198)]
  public class WeaponsGiver : BasePlugin {
    private ConVar tPrimary = null!;
    private ConVar tSecondary = null!;
    private ConVar tMelee = null!;
    private ConVar ctPrimary = null!;
    private ConVar ctSecondary = null!;
    private ConVar ctMelee = null!;

    public override string ModuleName => "WeaponsGiver";
    public override string ModuleAuthor => "ji";

    public override string ModuleDescription
      => "Ensures players in custom gamemodes spawn with starting weapons.";

    public override string ModuleVersion => "build7";

    public override void Load(bool hotReload) {
      RegisterEventHandler<EventPlayerSpawn>(Event_PlayerSpawn, HookMode.Post);
      RegisterEventHandler<EventRoundPrestart>(Event_RoundPrestart,
        HookMode.Pre);

      GetVars();
    }

    private void GetVars() {
      tPrimary    = ConVar.Find("mp_t_default_primary")!;
      tSecondary  = ConVar.Find("mp_t_default_secondary")!;
      tMelee      = ConVar.Find("mp_t_default_melee")!;
      ctPrimary   = ConVar.Find("mp_ct_default_primary")!;
      ctSecondary = ConVar.Find("mp_ct_default_secondary")!;
      ctMelee     = ConVar.Find("mp_ct_default_melee")!;
    }

    private HookResult Event_RoundPrestart(EventRoundPrestart @event,
      GameEventInfo info) {
      GetVars();
      return HookResult.Continue;
    }

    private HookResult Event_PlayerSpawn(EventPlayerSpawn @event,
      GameEventInfo info) {
      var player = @event.Userid;
      if (!player.IsValid
        || player.Connected != PlayerConnectedState.PlayerConnected)
        return HookResult.Continue;
      AddTimer(0.3f, () => GiveWeapons(player));
      return HookResult.Continue;
    }

    private void GiveWeapons(CCSPlayerController player) {
      if (!player.IsValid || !player.PlayerPawn.IsValid) return;
      if (player.Connected != PlayerConnectedState.PlayerConnected) return;
      if (player.LifeState != (int)LifeState_t.LIFE_ALIVE) return;

      player.RemoveWeapons();

      switch (player.Team) {
        case CsTeam.Terrorist:
          if (!string.IsNullOrEmpty(tPrimary.StringValue))
            player.GiveNamedItem(tPrimary.StringValue);
          if (!string.IsNullOrEmpty(tSecondary.StringValue))
            player.GiveNamedItem(tSecondary.StringValue);
          if (!string.IsNullOrEmpty(tMelee.StringValue))
            player.GiveNamedItem(tMelee.StringValue);
          break;

        case CsTeam.CounterTerrorist:
          if (!string.IsNullOrEmpty(ctPrimary.StringValue))
            player.GiveNamedItem(ctPrimary.StringValue);
          if (!string.IsNullOrEmpty(ctSecondary.StringValue))
            player.GiveNamedItem(ctSecondary.StringValue);
          if (!string.IsNullOrEmpty(ctMelee.StringValue))
            player.GiveNamedItem(ctMelee.StringValue);
          break;
        default:
          break;
      }
    }
  }
}