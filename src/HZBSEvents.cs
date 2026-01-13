using System;
using System.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mono.Cecil.Cil;
using Spectre.Console;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Helpers;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;
using SwiftlyS2.Shared.Sounds;
using static Dapper.SqlMapper;
using static HanZombieSounds.HZBSHelpers;
using static HZBSConfigs;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace HanZombieSounds;

public class HZBSEvents
{
    private readonly ILogger<HZBSEvents> _logger;
    private readonly ISwiftlyCore _core;
    private readonly IOptionsMonitor<HZBSConfigs> _config;
    private readonly HZBSHelpers _helpers;

    public bool[] InSwing { get; } = new bool[65];

    public HZBSEvents(ISwiftlyCore core, ILogger<HZBSEvents> logger,
        IOptionsMonitor<HZBSConfigs> config, HZBSHelpers helpers)
    {
        _core = core;
        _logger = logger;
        _config = config;
        _helpers = helpers;
    }

    public void HookEvents()
    {
        _core.Event.OnPrecacheResource += Event_OnPrecacheResource;
        _core.GameEvent.HookPre<EventPlayerHurt>(OnPlayerHurt);
        _core.GameEvent.HookPre<EventPlayerDeath>(OnPlayerDeath);
        _core.GameEvent.HookPre<EventPlayerHurt>(OnPlayerAttack);
        _core.GameEvent.HookPre<EventWeaponFire>(OnWeaponFire);
        _core.Event.OnEntityTakeDamage += Event_OnEntityTakeDamage;

        _core.Event.OnEntityTakeDamage += Event_OnInGrenadeDamage;

        _core.GameEvent.HookPre<EventRoundStart>(OnRoundStart);
        _core.Event.OnClientDisconnected += Event_OnClientDisconnected;
        _core.GameEvent.HookPre<EventPlayerSpawn>(OnPlayerSpawn);
    }

    private HookResult OnPlayerSpawn(EventPlayerSpawn @event)
    {
        var player = @event.UserIdPlayer;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        var controller = player.Controller;
        if (controller == null || !controller.IsValid)
            return HookResult.Continue;

        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        if (pawn.TeamNum != 2)
            return HookResult.Continue;

        var Entity = controller.Entity;
        if (Entity == null || !Entity.IsValid)
            return HookResult.Continue;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return HookResult.Continue;

        _core.Scheduler.DelayBySeconds(0.5f, () =>
        {
            var intervalMs = (int)(zombie.IdleInterval * 1000);
            var randomOffset = Random.Shared.Next(0, intervalMs);

            _helpers.g_ZombieIdleStates[player.PlayerID] = new ZombieIdleState
            {
                PlayerID = player.PlayerID,
                IdleInterval = zombie.IdleInterval,
                NextIdleTime = Environment.TickCount + intervalMs + randomOffset
            };
        });

        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event)
    {
        _core.Scheduler.DelayBySeconds(1.0f, () =>
        {
            GlobalIdleTimer();
        });
        return HookResult.Continue;
    }

    private void Event_OnInGrenadeDamage(IOnEntityTakeDamageEvent @event)
    {
        var victim = @event.Entity;
        if (victim == null || !victim.IsValid)
            return;

        var VictimPawn = victim.As<CCSPlayerPawn>();
        if (VictimPawn == null || !VictimPawn.IsValid)
            return;

        var VictimController = VictimPawn.Controller.Value?.As<CCSPlayerController>();
        if (VictimController == null || !VictimController.IsValid)
            return;

        var VictimPlayer = _core.PlayerManager.GetPlayer((int)(VictimController.Index - 1));
        if (VictimPlayer == null || !VictimPlayer.IsValid)
            return;

        if (VictimPawn.TeamNum != 2)
            return;

        var Entity = VictimController.Entity;
        if (Entity == null || !Entity.IsValid)
            return;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return;

        if (@event.Info.DamageType == DamageTypes_t.DMG_BURN)
        {
            _helpers.PlaySound(VictimPlayer, zombie.BurnSound, zombie.Volume);
        }
        else if (@event.Info.DamageType == DamageTypes_t.DMG_BLAST)
        {
            _helpers.PlaySound(VictimPlayer, zombie.ExplodeSound, zombie.Volume);
        }
    }

    private HookResult OnWeaponFire(EventWeaponFire @event)
    {
        var player = @event.UserIdPlayer;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        var controller = player.Controller;
        if (controller == null || !controller.IsValid)
            return HookResult.Continue;

        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        if (pawn.TeamNum != 2)
            return HookResult.Continue;

        if (!_helpers.IsPlayerUsingKnife(controller))
            return HookResult.Continue;

        var Entity = controller.Entity;
        if (Entity == null || !Entity.IsValid)
            return HookResult.Continue;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return HookResult.Continue;


        InSwing[player.PlayerID] = false;
        Task.Run(async () =>
        {
            await Task.Delay(50);
            if (!InSwing[player.PlayerID])
            {
                _helpers.PlaySound(player, zombie.SwingSound, zombie.Volume);
            }
        });

        return HookResult.Continue;
    }

    private void Event_OnEntityTakeDamage(IOnEntityTakeDamageEvent @event)
    {
        var attacker = @event.Info.Attacker.Value;
        if (attacker == null || !attacker.IsValid)
            return;

        var AttackerPawn = attacker.As<CCSPlayerPawn>();
        if (AttackerPawn == null || !AttackerPawn.IsValid)
            return;

        var AttackerController = AttackerPawn.Controller.Value?.As<CCSPlayerController>();
        if (AttackerController == null || !AttackerController.IsValid)
            return;

        if (AttackerPawn.TeamNum != 2)
            return;

        if (!_helpers.IsPlayerUsingKnife(AttackerController))
            return;

        var AttackerPlayer = _core.PlayerManager.GetPlayer((int)(AttackerController.Index - 1));
        if (AttackerPlayer == null || !AttackerPlayer.IsValid)
            return;

        var Entity = AttackerController.Entity;
        if (Entity == null || !Entity.IsValid)
            return;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return;

        if (@event.Entity.DesignerName != "worldent")
            return;

        InSwing[AttackerPlayer.PlayerID] = true;

        _helpers.PlaySound(AttackerPlayer, zombie.HitWallSound, zombie.Volume);

    }

    private HookResult OnPlayerAttack(EventPlayerHurt @event)
    {
        var attacker = _core.PlayerManager.GetPlayer(@event.Attacker);
        if (attacker == null || !attacker.IsValid)
            return HookResult.Continue;

        var controller = attacker.Controller;
        if (controller == null || !controller.IsValid)
            return HookResult.Continue;

        var Entity = controller.Entity;
        if (Entity == null || !Entity.IsValid)
            return HookResult.Continue;

        var pawn = attacker.PlayerPawn;
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        if (pawn.TeamNum != 2)
            return HookResult.Continue;

        if (!_helpers.IsPlayerUsingKnife(controller))
            return HookResult.Continue;

        InSwing[attacker.PlayerID] = true;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return HookResult.Continue;

        _helpers.PlaySound(attacker, zombie.HitSound, zombie.Volume);

        return HookResult.Continue;
    }
    private HookResult OnPlayerDeath(EventPlayerDeath @event)
    {
        var player = @event.UserIdPlayer;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        _helpers.g_ZombieIdleStates.Remove(player.PlayerID);

        var controller = player.Controller;
        if (controller == null || !controller.IsValid)
            return HookResult.Continue;

        var Entity = controller.Entity;
        if (Entity == null || !Entity.IsValid)
            return HookResult.Continue;

        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        if (pawn.TeamNum != 2)
            return HookResult.Continue;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return HookResult.Continue;

        _helpers.PlaySound(player, zombie.DieSound, zombie.Volume);

        return HookResult.Continue;
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event)
    {
        var attacker = _core.PlayerManager.GetPlayer(@event.Attacker);
        if (attacker == null || !attacker.IsValid)
            return HookResult.Continue;

        var attackercontroller = attacker.Controller;
        if (attackercontroller == null || !attackercontroller.IsValid)
            return HookResult.Continue;

        var attackerpawn = attacker.PlayerPawn;
        if (attackerpawn == null || !attackerpawn.IsValid)
            return HookResult.Continue;

        var player = @event.UserIdPlayer;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        var controller = player.Controller;
        if (controller == null || !controller.IsValid)
            return HookResult.Continue;

        var Entity = controller.Entity;
        if (Entity == null || !Entity.IsValid)
            return HookResult.Continue;

        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        if (pawn.TeamNum != 2 || pawn.TeamNum == attackerpawn.TeamNum || player == attacker)
            return HookResult.Continue;

        var cfg = _config.CurrentValue;
        var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
        if (zombie == null)
            return HookResult.Continue;

        bool isHeadshot = @event.HitGroup == 1;
        string soundPath = isHeadshot ? zombie.PainSound : zombie.HurtSound;

        _helpers.PlaySound(player, soundPath, zombie.Volume);

        return HookResult.Continue;

    }

    public void GlobalIdleTimer()
    {
        _helpers.g_IdleTimer?.Cancel();
        _helpers.g_IdleTimer = null;

        _helpers.g_IdleTimer = _core.Scheduler.RepeatBySeconds(0.1f, () =>
        {
            int now = Environment.TickCount;
            var aliveZombies = _core.PlayerManager.GetTAlive();
            foreach (var player in aliveZombies)
            {
                try
                {
                    if (!_helpers.g_ZombieIdleStates.TryGetValue(player.PlayerID, out var state))
                        continue;

                    var controller = player.Controller;
                    if (controller == null || !controller.IsValid)
                        continue;

                    var Entity = controller.Entity;
                    if (Entity == null || !Entity.IsValid)
                        continue;

                    if (!controller.PawnIsAlive)
                        continue;

                    if (now < state.NextIdleTime)
                        continue;

                    var cfg = _config.CurrentValue;
                    var zombie = cfg.ZombieList.FirstOrDefault(z => z.Enable && z.Name == Entity.Name);
                    if (zombie == null)
                        continue;

                    _helpers.PlaySound(player, zombie.IdleSound, zombie.Volume);

                    state.NextIdleTime = now + (int)(state.IdleInterval * 1000);
                }
                catch (Exception ex)
                {
                    _core.Logger.LogError($"Idle Error: {ex.Message}");
                }

            }
        });

        _core.Scheduler.StopOnMapChange(_helpers.g_IdleTimer);
    }

    private void Event_OnPrecacheResource(SwiftlyS2.Shared.Events.IOnPrecacheResourceEvent @event)
    {
        var cfg = _config.CurrentValue;
        var soundevent = cfg.ZombieList;
        foreach (var sound in soundevent)
        {
            @event.AddItem(sound.PrecacheSounds);
        }
    }

    private void Event_OnClientDisconnected(IOnClientDisconnectedEvent @event)
    {
        var client = @event.PlayerId;

        _helpers.g_ZombieIdleStates.Remove(client);
    }
}