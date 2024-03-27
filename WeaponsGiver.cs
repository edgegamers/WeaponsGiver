using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace WeaponsGiver
{
    [MinimumApiVersion(198)]
    public class WeaponsGiver : BasePlugin
    {
        private string tPrimary = "";
        private string tSecondary = "";
        private string tMelee = "";
        private string ctPrimary = "";
        private string ctSecondary = "";
        private string ctMelee = "";

        public override string ModuleName => "WeaponsGiver";
        public override string ModuleAuthor => "ji";
        public override string ModuleDescription => "Ensures players in custom gamemodes spawn with starting weapons.";
        public override string ModuleVersion => "build7";

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventPlayerSpawn>(Event_PlayerSpawn, HookMode.Post);
            RegisterEventHandler<EventRoundPrestart>(Event_RoundPrestart, HookMode.Pre);
        }

        private void GetVars()
        {
            tPrimary = ConVar.Find("mp_t_default_primary")?.StringValue ?? "";
            tSecondary = ConVar.Find("mp_t_default_secondary")?.StringValue ?? "";
            tMelee = ConVar.Find("mp_t_default_melee")?.StringValue ?? "";
            ctPrimary = ConVar.Find("mp_ct_default_primary")?.StringValue ?? "";
            ctSecondary = ConVar.Find("mp_ct_default_secondary")?.StringValue ?? "";
            ctMelee = ConVar.Find("mp_ct_default_melee")?.StringValue ?? "";
        }

        private HookResult Event_RoundPrestart(EventRoundPrestart @event, GameEventInfo info)
        {
            GetVars();
            return HookResult.Continue;
        }

        private HookResult Event_PlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if(!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
                return HookResult.Continue;
            Server.RunOnTick(Server.TickCount + 1, () => GiveWeapons(player));
            return HookResult.Continue;
        }

        private void GiveWeapons(CCSPlayerController player)
        {
            if(!player.IsValid || !player.PlayerPawn.IsValid) return;
            if (player.Connected != PlayerConnectedState.PlayerConnected) return;
            
            player.RemoveWeapons();

            switch(player.Team)
            {
                case CsTeam.Terrorist:
                    if(!string.IsNullOrEmpty(tPrimary)) player.GiveNamedItem(tPrimary);
                    if(!string.IsNullOrEmpty(tSecondary)) player.GiveNamedItem(tSecondary);
                    if(!string.IsNullOrEmpty(tMelee)) player.GiveNamedItem(tMelee);
                    break;
                
                case CsTeam.CounterTerrorist:
                    if(!string.IsNullOrEmpty(ctPrimary)) player.GiveNamedItem(ctPrimary);
                    if(!string.IsNullOrEmpty(ctSecondary)) player.GiveNamedItem(ctSecondary);
                    if(!string.IsNullOrEmpty(ctMelee)) player.GiveNamedItem(ctMelee);
                    break;
            }
        }
    }
}
