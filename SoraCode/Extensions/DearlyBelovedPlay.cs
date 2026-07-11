using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Saves;

namespace Sora.SoraCode.Extensions;

public class DearlyBelovedPlay
{
    private const string MusicPath = "res://Sora/sfx/Dearly_Beloved.mp3";
    private static AudioStreamPlayer? _player;

    public static void Play()
    {
        // Always restart from the beginning
        Stop();

        NAudioManager.Instance?.SetBgmVol(0f);

        var stream = GD.Load<AudioStream>(MusicPath);

        if (stream == null)
            return;

        _player = new AudioStreamPlayer
        {
            Stream = stream,
            Name = "SoraCharacterSelectMusic",
            Bus = "SFX"
        };

        NGame.Instance?.AddChild(_player);

        _player.Play(); // starts at 0 every time
    }

    public static void Stop()
    {
        if (_player != null && GodotObject.IsInstanceValid(_player))
        {
            _player.Stop();
            _player.QueueFree();
        }

        _player = null;

        if (SaveManager.Instance?.SettingsSave != null)
        {
            NAudioManager.Instance?.SetBgmVol(
                SaveManager.Instance.SettingsSave.VolumeBgm
            );
        }
    }

    }

[HarmonyPatch(typeof(NCharacterSelectScreen))]
public static class CharacterSelectMusicPatch
{
    [HarmonyPatch(nameof(NCharacterSelectScreen.SelectCharacter))]
    [HarmonyPostfix]
    private static void SelectCharacter_Postfix(
        NCharacterSelectButton charSelectButton,
        CharacterModel characterModel)
    {
        if (characterModel is Character.Sora)
        {
            DearlyBelovedPlay.Play();
        }
        else
        {
            DearlyBelovedPlay.Stop();
        }
    }

    [HarmonyPatch(nameof(NCharacterSelectScreen.OnSubmenuClosed))]
    [HarmonyPrefix]
    private static void OnSubmenuClosed_Prefix()
    {
        DearlyBelovedPlay.Stop();
    }

    [HarmonyPatch(nameof(NCharacterSelectScreen.BeginRun))]
    [HarmonyPrefix]
    private static void BeginRun_Prefix()
    {
        DearlyBelovedPlay.Stop();
    }
    
}
[HarmonyPatch(typeof(NCharacterSelectScreen))]
public static class SoraEmbarkPatch
{
    private static bool _isRunning;
    private static AudioStreamPlayer? _voicePlayer;
    [HarmonyPatch("OnEmbarkPressed")]
    [HarmonyPrefix]
    public static bool Prefix(NCharacterSelectScreen __instance, NButton _)
    {
        
        if (_isRunning)
        {
            _isRunning = false;
            return true;
        }

        var lobby = (StartRunLobby)AccessTools
            .Field(typeof(NCharacterSelectScreen), "_lobby")
            .GetValue(__instance);

        // Leave multiplayer untouched
        if (lobby.NetService.Type != NetGameType.Singleplayer)
            return true;

        if (lobby.LocalPlayer.character is not Character.Sora)
            return true;

        _isRunning = true;

        TaskHelper.RunSafely(DelayedEmbark(__instance));

        return false;
    }

    private static async Task DelayedEmbark(NCharacterSelectScreen screen)
    {
        try
        {
            // Disable embark button
            var embarkButton = (NConfirmButton)AccessTools
                .Field(typeof(NCharacterSelectScreen), "_embarkButton")
                .GetValue(screen);

            embarkButton.Disable();

            // Disable back button
            var backButton = (NBackButton)AccessTools
                .Field(typeof(NCharacterSelectScreen), "_backButton")
                .GetValue(screen);

            backButton.Disable();

            // Disable character buttons
            var charContainer = (Control)AccessTools
                .Field(typeof(NCharacterSelectScreen), "_charButtonContainer")
                .GetValue(screen);

            foreach (var button in charContainer
                         .GetChildren()
                         .OfType<NCharacterSelectButton>())
            {
                button.Disable();
            }

            // Hide character selection UI
            charContainer.Visible = false;

            SfxCmd.Play("res://Sora/sfx/game_start.mp3", 1f);
            // Wait 4 seconds
            await Cmd.Wait(4f);

            // Continue normal embark
            AccessTools.Method(
                    typeof(NCharacterSelectScreen),
                    "OnEmbarkPressed")
                ?.Invoke(screen, new object[] { null });
        }
        finally
        {
            _isRunning = false;
        }
    }
}
        
        


    