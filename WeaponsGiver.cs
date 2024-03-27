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
        private string tPrimary;
        private string tSecondary;
        private string tMelee;
        private string ctPrimary;
        private string ctSecondary;
        private string ctMelee;

        public override string ModuleName => "WeaponsGiver";
        public override string ModuleAuthor => "ji";
        public override string ModuleDescription => "Ensures players in custom gamemodes spawn with starting weapons.";
        public override string ModuleVersion => "build6";

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventPlayerSpawn>(Event_PlayerSpawn, HookMode.Post);
            RegisterEventHandler<EventRoundPrestart>(Event_RoundPrestart, HookMode.Pre);
        }

        public void GetVars()
        {
            tPrimary = ConVar.Find("mp_t_default_primary").StringValue;
            tSecondary = ConVar.Find("mp_t_default_secondary").StringValue;
            tMelee = ConVar.Find("mp_t_default_melee").StringValue;
            ctPrimary = ConVar.Find("mp_ct_default_primary").StringValue;
            ctSecondary = ConVar.Find("mp_ct_default_secondary").StringValue;
            ctMelee = ConVar.Find("mp_ct_default_melee").StringValue;
            
        }

        private HookResult Event_RoundPrestart(EventRoundPrestart @event, GameEventInfo info)
        {
            GetVars();
            return HookResult.Continue;
        }

        private HookResult Event_PlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            Server.RunOnTick(Server.TickCount + 1, () => GiveWeapons(@event.Userid));
            return HookResult.Continue;
        }

        private void GiveWeapons(CCSPlayerController player)
        {
            if(!player.IsValid || !player.PlayerPawn.IsValid) return;
            if (player.Connected != PlayerConnectedState.PlayerConnected) return;

            switch((CsTeam)player.TeamNum)
            {
                case CsTeam.Terrorist:
                    if(!String.IsNullOrEmpty(tPrimary)) player.GiveNamedItem(tPrimary);
                    if(!String.IsNullOrEmpty(tSecondary)) player.GiveNamedItem(tSecondary);
                    if(!String.IsNullOrEmpty(tMelee)) player.GiveNamedItem(tMelee);
                    break;
                
                case CsTeam.CounterTerrorist:
                    if(!String.IsNullOrEmpty(ctPrimary)) player.GiveNamedItem(ctPrimary);
                    if(!String.IsNullOrEmpty(ctSecondary)) player.GiveNamedItem(ctSecondary);
                    if(!String.IsNullOrEmpty(ctMelee)) player.GiveNamedItem(ctMelee);
                    break;
            }
        }
    }
}
