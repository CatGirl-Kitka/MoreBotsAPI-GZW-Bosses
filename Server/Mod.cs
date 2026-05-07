using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using System.Reflection;
using MoreBotsServer.Services;
using SPTarkov.Server.Core.Models.Eft.Common;
using WTTServerCommonLib;

namespace MoreBotsServerExample;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.morebotsapiexample.tacticaltoaster";
    public override string Name { get; init; } = "MoreBotsAPIExample";
    public override string Author { get; init; } = "TacticalToaster";
    public override List<string>? Contributors { get; init; } = new() { };
    public override SemanticVersioning.Version Version { get; init; } = new(2, 0, 0);
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new()
    {
        { "com.morebotsapi.tacticaltoaster", new SemanticVersioning.Range(">=1.0.0") }
    };
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

// Ensure bots are loaded with the predefined load order
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = MoreBotsServer.MoreBotsLoadOrder.LoadBots)]
public class MoreBotsExample(
    MoreBotsServer.MoreBotsAPI moreBotsLib,
    FactionService factionService,
    MoreBotsCustomBotTypeService customBotTypeService,
    WTTServerCommonLib.WTTServerCommonLib commonLib
) : IOnLoad
{
    public async Task OnLoad()
    {
        //              //
        // LOADING BOTS //
        //              //
        
        // WildSpawnType int, WildSpawnType name
        var typeDictionary = new Dictionary<int, string>()
        {
            { 1069, "bossExampleBot" }
        };

        // This list is used in some methods found in the API
        List<string> typeList = typeDictionary.Values.ToList();
        
        // Loads bots based on the file name in the db/bots/types and db/bots/config folder
        await moreBotsLib.LoadBots(Assembly.GetExecutingAssembly());
        
        // If a WildSpawnType enum doesn't exist by default in SPT/EFT, the API will use an internal dictionary to translate the names/values
        // This adds to that internal dictionary
        customBotTypeService.AddCustomWildSpawnTypeNames(typeDictionary);
        
        // The API includes some custom ways to load settings and loadouts, including replacing existing ones.
        // I use the replace methods to overwrite clothing, loadouts, and settings if certain mods are installed.
        // In addition, there is info that is defined outside of the bot type values for things such as wallet contents, armor and equipment use/durability settings, etc.
        
        
        //                  //
        // FACTION SETTINGS //
        //                  //
        
        /* Default Factions
         * Factions are composed of bot types and subfactions. Bots in subfactions are also considered a part of the faction that contains the subfactions.
         * 
         * scavs - normal scavs, scav snipers, and some misc event bots
         * raiders
         * rogues - includes Goons
         * smugglers - arenaFighterEvent
         * bloodhounds - arenaFighter
         * cultists - includes event cultists and Zryachiy
         * infected - all zombies
         * usec - pmcUSEC
         * bear - pmcBEAR
         * pmcs - usec and bear are subfactions
         * killaTagilla - includes labryinth variants, the labryinth followers, and the unused followerTagilla
         * kabanKolontay - includes guards of both
         * reshala - includes guards
         * shturman - includes guards
         * gluhar - includes guards
         * sanitar - includes guards
         * partisan - he's lonely :(
         * misc - btr and santa :D
         * scavbosses - all normal boss factions are subfactions, partisan not included
         * criminals - scavs and scavbosses are subfactions
         * savage - scavs, scavbosses, smugglers, bloodhounds, and raiders are subfactions
         */
        
        // Make bots in the first faction hostile to those in the second
        factionService.AddEnemyByFaction("exampleFaction", "criminals");
        factionService.AddEnemyByFaction("exampleFaction", "cultists");
        factionService.AddEnemyByFaction("criminals", "exampleFaction");
        factionService.AddEnemyByFaction("cultists", "exampleFaction");
        
        // Make bots in the first faction friendly to those in the second
        factionService.AddFriendlyByFaction("exampleFaction", "rogues");
        factionService.AddFriendlyByFaction("rogues", "exampleFaction");
        
        // Make bots in the first faction avenge those killed in the second (kill faction 2, bots in faction 1 become hostile to whoever did the kill)
        factionService.AddRevengeByFaction("exampleFaction", "rogues");
        factionService.AddRevengeByFaction("rogues", "exampleFaction");
        
        // Make bots in the first faction attempt to warn bots in the second before becoming hostile, requires bot to have warn behavior enabled
        factionService.AddWarnByFaction("exampleFaction", "raiders");
        factionService.AddWarnByFaction("raiders", "exampleFaction");
        
        // You can modify the faction hostility after being killed in a raid
        factionService.SetRevengeAfterRaids("exampleFaction", true, 3);

        await commonLib.CustomLocaleService.CreateCustomLocales(Assembly.GetExecutingAssembly());
        
        await Task.CompletedTask;
    }
}

// Ensure factions are loaded with the predefined load order
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = MoreBotsServer.MoreBotsLoadOrder.LoadFactions)]
public class MoreBotsExampleFaction(
    MoreBotsServer.MoreBotsAPI moreBotsLib,
    FactionService factionService
) : IOnLoad
{
    public async Task OnLoad()
    {
        var exampleFaction = new Faction()
        {
            // Name of faction
            Name = "exampleFaction",
            // Bot types included in the faction
            // Custom bots must be casted to WildSpawnType using the int value of the custom WildSpawnType
            // (WildSpawnType)intValue
            BotTypes =
            {
                (WildSpawnType)1069
            },
            // Will the faction be hostile to players for a number of raids if they kill a member?
            RevengeAfterRaids = true,
            // How many raids will they be hostile for?
            RevengeRaidAmount = 3
        };
        
        // Adds a faction to the faction service
        factionService.Factions.Add(exampleFaction.Name, exampleFaction);
        
        await Task.CompletedTask;
    }
}