using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using System.Reflection;

namespace MoreBotsServerExample;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.morebotsapiexample.tacticaltoaster";
    public override string Name { get; init; } = "MoreBotsAPIExample";
    public override string Author { get; init; } = "TacticalToaster";
    public override List<string>? Contributors { get; init; } = new() { };
    public override SemanticVersioning.Version Version { get; init; } = new(1, 0, 0);
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

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public class MoreBotsExample(
    MoreBotsServer.MoreBotsLib moreBotsLib
) : IOnLoad
{
    public async Task OnLoad()
    {
        await moreBotsLib.LoadBots(Assembly.GetExecutingAssembly());

        await Task.CompletedTask;
    }
}