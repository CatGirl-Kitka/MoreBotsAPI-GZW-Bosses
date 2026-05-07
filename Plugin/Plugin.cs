using BepInEx;
using EFT;
using MoreBotsAPI.Components;
using MoreBotsAPIExample;

namespace MoreBotsAPIExample;

// We want these to always load before our plugin in case they're being used
[BepInDependency("xyz.drakia.bigbrain", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("me.sol.sain", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(ClientInfo.GUID, ClientInfo.PluginName, ClientInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Creates a list of the custom WildSpawnType enums
        var botEnums = new List<int> { 1069 }
            .ConvertAll(x => (WildSpawnType)x);
        
        //              //
        // BOT HUNTS    //
        //              //
        
        /*
         * Bot hunts only occur if bots are configured here to hunt specific types/player sides,
         * and if they are spawned with certain parameters.
         * The bots must spawn as part of a boss group configured with TriggerType of "botEvent"
         * and TriggerId of "hunt" (or an Id that contains the word "hunt").
         * The API calls the "hunt" bot event at the start of every raid, so all other parameters,
         * including spawn chance, should work as expected.
         * Bots spawned in groups that don't have the TriggerType and TriggerId set will not use
         * hunt behaviors.
         * The plugin API allows for manual initialization and enabling of hunt behaviors
         * for special cases and functionality.
         */
        
        // Bot hunt groups of the first argument will hunt bots of the second argument
        MonoBehaviourSingleton<HuntManager>.Instance.AddHuntRoles(botEnums, [WildSpawnType.bossBully, WildSpawnType.sectantPriest]);
        // Bot hunt groups of the first argument will hunt players of the sides in the second argument
        MonoBehaviourSingleton<HuntManager>.Instance.AddHuntSides(botEnums, new List<EPlayerSide> { EPlayerSide.Usec, EPlayerSide.Bear });

        // When the bot hunt manager of an individual bot is activated, this event is called. The parameter is the individual bot manager.
        // You can use this to test the 
        MonoBehaviourSingleton<HuntManager>.Instance.OnBotHuntInit += manager =>
        {
            if (botEnums.Contains(manager.botOwner.Profile.Info.Settings.Role))
            {
                // Do something for our custom bots when bot hunt manager is initiated
                
                // For example, add to the priority target list
                // manager.priorityTargets.Add();
                
                // This list doesn't have to abide by the hunt roles and sides previously defined
                
                // Hunters will prioritize whoever is in their priorityTargets list,
                // only targeting bots defined by their normal settings once priorityTargets are dead/gone
            }
        };
    }
}