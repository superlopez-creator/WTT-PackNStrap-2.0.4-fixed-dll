using System.Collections.Generic;
using SemanticVersioning;
using SPTarkov.Server.Core.Models.Spt.Mod;
using WTTPackNStrap;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; }
    public override string Name { get; init; }
    public override string Author { get; init; }
    public override List<string>? Contributors { get; init; }
    public override Version Version { get; init; }
    public override Range SptVersion { get; init; }
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; }
    public ModMetadata()
    {
        ModGuid = "com.wtt.packnstrap";
        Name = "WTT-PackNStrapServer";
        Author = "GrooveypenguinX";
        Version = new Version(typeof(ModMetadata).Assembly.GetName().Version?.ToString(3), false);
        SptVersion = new Range("~4.0.2", false);
        ModDependencies = new Dictionary<string, Range> { { "com.wtt.commonlib", new Range("~2.0.0", false) } };
        IsBundleMod = true;
        License = "MIT";
        base..ctor();
    }
}
