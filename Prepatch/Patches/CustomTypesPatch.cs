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

        public static AssemblyDefinition patchAssembly;

        public static void Patch(ref AssemblyDefinition assembly)
        {
            patchAssembly = assembly;

            Logger.CreateLogSource("MoreBotsAPIExample").LogInfo("Creating bossExampleBot!");

            var exampleBot = new CustomWildSpawnType(1069, "bossExampleBot", "Boss", 32, true, false, false);

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

            CustomWildSpawnTypeManager.RegisterWildSpawnType(exampleBot, assembly); // This is what registers your new spawn type, adding it to the WildSpawnType enum and .

            CustomWildSpawnTypeManager.AddSuitableGroup(new List<int> { 1069, 1 }); // This allows the example bot to be the boss in a group with itself or normal scavs.
        }

    }
}