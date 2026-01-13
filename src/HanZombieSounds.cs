using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mono.Cecil.Cil;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace HanZombieSounds;

[PluginMetadata(
    Id = "HanZombieSounds",
    Version = "1.0.0",
    Name = "ZombieSounds",
    Author = "H-AN",
    Description = "自定义丧尸各项音效/custom zombie sounds")]
public partial class HanZombieSounds(ISwiftlyCore core) : BasePlugin(core)
{
    private ServiceProvider? ServiceProvider { get; set; }
    private HZBSConfigs _CFG = null!;
    private HZBSEvents _Events = null!;
    public override void Load(bool hotReload)
    {
        Core.Configuration.InitializeJsonWithModel<HZBSConfigs>("HanZombieSoundCFG.jsonc", "ZombieSounds").Configure(builder =>
        {
            builder.AddJsonFile("HanZombieSoundCFG.jsonc", false, true);
        });

        var collection = new ServiceCollection();
        collection.AddSwiftly(Core);

        collection
            .AddOptionsWithValidateOnStart<HZBSConfigs>()
            .BindConfiguration("ZombieSounds");

        collection.AddSingleton<HZBSHelpers>();
        collection.AddSingleton<HZBSEvents>();

        ServiceProvider = collection.BuildServiceProvider();

        _Events = ServiceProvider.GetRequiredService<HZBSEvents>();

        var monitor = ServiceProvider.GetRequiredService<IOptionsMonitor<HZBSConfigs>>();

        _CFG = monitor.CurrentValue;

        monitor.OnChange(newConfig =>
        {
            _CFG = newConfig;
            Core.Logger.LogInformation("[HanZombieSoundCFG] hot load success!");
        });

        _Events.HookEvents();
    }

    public override void Unload()
    {
        ServiceProvider!.Dispose();
    }

}