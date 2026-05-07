using BepInEx.Logging;
using Mono.Cecil;
using MoreBotsAPI;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MoreBotsAPIExample.Prepatch
{
    public static class WildSpawnTypePatch
    {
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        public static void Patch(ref AssemblyDefinition assembly)
        {
            // 1069 is the enum value for your custom type, followed by the enum name. The other variables are explained in the CustomWildSpawnType class comments
            var exampleBot = new CustomWildSpawnType(1069, "bossExampleBot", "Example", 32, true, false, false);

            exampleBot.SetCountAsBossForStatistics(true);
            // Should having max fence loyalty stop this bot from warning your pscav. Doesn't affect hostility (that is defined in the type json), only interaction with warn behavior.
            exampleBot.SetShouldUseFenceNoBossAttack(true, false);
            // Exclude all difficulties except Normal. This is done by default if you do not set excluded difficulties.
            // 0 = easy, 1 = normal, 2 = hard, 3 = impossible
            exampleBot.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            SAINSettings settings = new SAINSettings(exampleBot.WildSpawnTypeValue)
            {
                Name = "Example Bot",
                Description = "An example bot created using MoreBotsAPI.",
                // This is the bot section your bot will appear under, like Bosses, Followers, PMCs, etc.
                Section = "Custom",
                // Not used unless you didn't define BrainsToApply
                BaseBrain = "Assault",
                // Look for the ShortName in the class of the brains you want to apply to.
                BrainsToApply = new List<string> { "Assault" },
                // SAIN difficulty modifier for the bot, many bosses have values above .5f.
                DifficultyModifier = .5f,
                // When SAIN layers are applied, these vanilla layers should be removed
                // Some common vanilla layers will always be removed to not conflict with SAIN
                LayersToRemove = new List<string>() {}
            };

            exampleBot.SetSAINSettings(settings);

            // This is what registers your new spawn type, adding it to the WildSpawnType enum and manager for custom bot types.
            CustomWildSpawnTypeManager.RegisterWildSpawnType(exampleBot, assembly);

            // This allows the example bot to be the boss in a group with itself or normal scavs.
            // If you want your bot to be solo you don't have to worry about this.
            CustomWildSpawnTypeManager.AddSuitableGroup(new List<int> { 1069, 1 });
        }

    }
}