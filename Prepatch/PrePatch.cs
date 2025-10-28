using BepInEx;
using MoreBotsAPI;
using System.Collections.Generic;

namespace MoreBotsAPIExample
{
    [BepInDependency("com.morebotsapi.tacticaltoaster", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ClientInfo.PreLoadGUID, ClientInfo.PreLoadName, ClientInfo.Version)]
    public class MoreBotsPrepatch : BaseUnityPlugin
    {
        public static MoreBotsPrepatch Instance;

        public void Awake()
        {
            var exampleBot = new CustomWildSpawnType(1069, "bossExampleBot", "Boss", 1, true, false, false);

            exampleBot.SetCountAsBossForStatistics(true);
            exampleBot.SetShouldUseFenceNoBossAttack(true, false);
            exampleBot.SetExcludedDifficulties(new List<int> { 0, 2, 3 }); // Exclude all difficulties except Normal. This is done by default if you do not set excluded difficulties.
            
            SAINSettings settings = new SAINSettings(exampleBot.WildSpawnTypeValue)
            {
                Name = "Example Bot",
                Description = "An example bot created using MoreBotsAPI.",
                Section = "Custom",
                BaseBrain = "Assault",
                BrainsToApply = new List<string> { "Assault" }, // Look for the ShortName in the class of the brains you want to apply to.
            };

            exampleBot.SetSAINSettings(settings);

            CustomWildSpawnTypeManager.AddType(exampleBot);

            CustomWildSpawnTypeManager.AddSuitableGroup(new List<int> { 1069, 1 }); // This allows the example bot to be the boss in a group with itself or normal scavs.
        }
    }
}
