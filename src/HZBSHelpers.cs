using System.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Helpers;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace HanZombieSounds;

public class HZBSHelpers
{
    private readonly ILogger<HZBSHelpers> _logger;
    private readonly ISwiftlyCore _core;

    public Dictionary<int, ZombieIdleState> g_ZombieIdleStates = new();
    public CancellationTokenSource? g_IdleTimer { get; set; } = null;

    public class ZombieIdleState
    {
        public int PlayerID;
        public float IdleInterval;   // 间隔秒数
        public float NextIdleTime;   // 下一次Idle时间
    }

    public HZBSHelpers(ISwiftlyCore core, ILogger<HZBSHelpers> logger)
    {
        _core = core;
        _logger = logger;
    }

    public void EmitSoundToPlayer(IPlayer player, string SoundEvent, float volume)
    {
        if (!string.IsNullOrEmpty(SoundEvent))
        {
            var sound = new SwiftlyS2.Shared.Sounds.SoundEvent(SoundEvent, volume, 1.0f);
            sound.SourceEntityIndex = (int)player.Controller.Index;
            sound.Recipients.AddAllPlayers();
            _core.Scheduler.NextTick(() => { sound.Emit(); });
        }
    }

    public string? RandomSelectSound(string sound)
    {
        if (string.IsNullOrWhiteSpace(sound)) return null;

        var items = sound
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();

        if (items.Length == 0) return null;

        return items.Length == 1 ? items[0] : items[Random.Shared.Next(items.Length)];
    }

    public bool IsPlayerUsingKnife(CCSPlayerController controller)
    {
        if (controller == null || !controller.IsValid)
            return false;

        var pawn = controller.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid)
            return false;

        var weaponServices = pawn.WeaponServices;
        if (weaponServices == null || !weaponServices.IsValid)
            return false;

        var activeWeapon = weaponServices.ActiveWeapon.Value;
        if (activeWeapon == null || !activeWeapon.IsValid)
            return false;

        return activeWeapon.DesignerName == "weapon_knife";
    }

    public void PlaySound(IPlayer player, string soundPath, float volume)
    {
        if (string.IsNullOrWhiteSpace(soundPath))
            return;

        var sound = RandomSelectSound(soundPath);
        if (string.IsNullOrWhiteSpace(sound))
            return;

        EmitSoundToPlayer(player, sound, volume);
    }




}
