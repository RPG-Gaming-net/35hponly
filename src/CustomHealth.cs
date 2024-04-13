namespace CustomHealth;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text.Json.Serialization;

public class Config : BasePluginConfig
{
    [JsonPropertyName("set-player-health")]
    public int SetPlayerHealth { get; set; } = 35;
}
public partial class CustomHealth : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "CustomHealth";
    public override string ModuleVersion => "1.0.3";
    public override string ModuleAuthor => "_audio / The Bowered [NL]";

    public required Config Config { get; set; }
    public void OnConfigParsed(Config config)
    {
        Config = config;
    }
    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;

        if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid)
        {
            return HookResult.Continue;
        }

        var playerPawn = player.PlayerPawn.Value;

        if (playerPawn.LifeState != (byte)LifeState_t.LIFE_ALIVE || player.Connected != PlayerConnectedState.PlayerConnected)
        {
            return HookResult.Continue;
        }

        if (playerPawn.Health > Config.SetPlayerHealth)
        {
            Server.NextFrame(() =>
            {
                AddTimer(0.75f, () =>
                {
                    playerPawn.Health = Config.SetPlayerHealth;
                    Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_iHealth");
                });
            });
        }

        return HookResult.Continue;
    }
}