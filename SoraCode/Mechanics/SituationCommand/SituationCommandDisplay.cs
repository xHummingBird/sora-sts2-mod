using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Mechanics.SituationCommand;

public partial class SituationCommandDisplay : Control
{
    public static SituationCommandDisplay? Instance { get; private set; }

    private Control? _display;

    private TextureRect? _command1;
    private TextureRect? _command2;
    private TextureRect? _command3;

    private RichTextLabel? _command1Label;
    private RichTextLabel? _command2Label;
    private RichTextLabel? _command3Label;

    private Player? _player;

    private string _lastCommand1Text = string.Empty;
    private string _lastCommand2Text = string.Empty;
    private string _lastCommand3Text = string.Empty;

    private Tween? _command1Tween;
    private Tween? _command2Tween;
    private Tween? _command3Tween;
    
    private const string SonicBladeKey = "SORA-SONIC_BLADE.title";
    private const string ArsArcanumKey = "SORA-ARS_ARCANUM.title";
    
    
    private const bool DebugForceVisible = false;

    public override void _Ready()
    {
        Instance = this;

        Name = "SituationCommandDisplay";

        MouseFilter = MouseFilterEnum.Pass;

        CallDeferred(nameof(Setup));
    }

    private async void Setup()
    {
        if (!IsInsideTree())
            return;

        for (int i = 0; i < 60; i++)
        {
            var state =
                CombatManager.Instance?.DebugOnlyGetState();

            var player =
                state?.Players.FirstOrDefault(
                    p => LocalContext.IsMe(p));

            if (player != null)
            {
                if (player.Character is not Character.Sora)
                {
                    QueueFree();
                    return;
                }

                _player = player;
                break;
            }

            var tree = GetTree();

            if (tree == null)
                return;

            await ToSignal(
                tree,
                SceneTree.SignalName.ProcessFrame);
        }

        if (_player == null)
        {
            QueueFree();
            return;
        }

        var scene =
            GD.Load<PackedScene>(
                "res://Sora/scenes/command_display.tscn");

        if (scene == null)
        {
            GD.PushError(
                "[Sora Command] Failed to load command_display.tscn");

            QueueFree();
            return;
        }

        _display =
            scene.Instantiate<Control>();

        AddChild(_display);

        _display.MouseFilter =
            MouseFilterEnum.Pass;

        _display.SetAnchorsPreset(
            LayoutPreset.BottomLeft);

        /*
         * Tweak this position until it lines up with your KH-style HUD.
         * X positive = move right
         * Y negative = move upward
         */
        _display.Position =
            new Vector2(
                -125,
                -120);

        _command1 =
            _display.GetNodeOrNull<TextureRect>(
                "Command1");

        _command2 =
            _display.GetNodeOrNull<TextureRect>(
                "Command2");

        _command3 =
            _display.GetNodeOrNull<TextureRect>(
                "Command3");

        _command1Label =
            _display.GetNodeOrNull<RichTextLabel>(
                "Command1/CommandLabel1");

        _command2Label =
            _display.GetNodeOrNull<RichTextLabel>(
                "Command2/CommandLabel2");

        _command3Label =
            _display.GetNodeOrNull<RichTextLabel>(
                "Command3/CommandLabel3");
        ApplyCommandFont(_command1Label);
        ApplyCommandFont(_command2Label);
        ApplyCommandFont(_command3Label);

        if (_command1 == null ||
            _command2 == null ||
            _command3 == null ||
            _command1Label == null ||
            _command2Label == null ||
            _command3Label == null)
        {
            GD.PushError(
                "[Sora Command] command_display.tscn must contain Command1, Command2, Command3, each with CommandLabel child.");

            QueueFree();
            return;
        }

        HideAllCommands();

        CallDeferred(nameof(UpdateCommandPivots));

        UpdateDisplay();
    }
    
    private static void ApplyCommandFont(
        RichTextLabel? label)
    {
        if (label == null)
            return;

        var font =
            GD.Load<Font>(
                "res://themes/kreon_regular_shared.tres");

        if (font != null)
        {
            label.AddThemeFontOverride(
                "font",
                font);

            label.AddThemeFontOverride(
                "normal_font",
                font);
        }

        label.AddThemeColorOverride(
            "default_color",
            Colors.White);

        label.AddThemeColorOverride(
            "font_outline_color",
            Colors.Black);

        label.AddThemeConstantOverride(
            "outline_size",
            8);

        label.AddThemeFontSizeOverride(
            "normal_font_size",
            24);

        label.HorizontalAlignment =
            HorizontalAlignment.Left;

        label.Position =
            new Vector2(
                90,
                -2);
    }

    private void UpdateCommandPivots()
    {
        SetPivotToCenter(_command1);
        SetPivotToCenter(_command2);
        SetPivotToCenter(_command3);
    }

    private static void SetPivotToCenter(
        TextureRect? command)
    {
        if (command == null)
            return;

        command.PivotOffset =
            command.Size / 2f;
    }

    public override void _Process(
        double delta)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_player == null)
            return;

        if (_command1 == null ||
            _command2 == null ||
            _command3 == null ||
            _command1Label == null ||
            _command2Label == null ||
            _command3Label == null)
        {
            return;
        }
        
        if (DebugForceVisible)
        {
            ForceShowTestCommands();
            return;
        }

        var relic =
            _player.Relics
                .OfType<SituationRelicBase>()
                .FirstOrDefault();

        if (relic == null)
        {
            HideAllCommands();
            return;
        }

        List<string> commands =
            GetCurrentCommands(
                relic);

        ApplyCommands(
            commands);
    }
    
    private void ForceShowTestCommands()
    {
        _command1!.Visible = true;
        _command2!.Visible = true;
        _command3!.Visible = true;

        _command1Label!.Visible = true;
        _command2Label!.Visible = true;
        _command3Label!.Visible = true;

        _command1Label.Text = "Sonic Blade";
        _command2Label.Text = "Ars Arcanum";
        _command3Label.Text = "TEST COMMAND";

        _command1.Modulate = Colors.White;
        _command2.Modulate = Colors.White;
        _command3.Modulate = Colors.White;

        _command1.Scale = Vector2.One;
        _command2.Scale = Vector2.One;
        _command3.Scale = Vector2.One;
    }

    private List<string> GetCurrentCommands(
        SituationRelicBase relic)
    {
        List<string> commands = [];

        if (relic.SituationPoints >= 60)
        {
            commands.Add(ArsArcanumKey);
        }
        else if (relic.SituationPoints >= 30)
        {
            commands.Add(SonicBladeKey);
        }

        return commands;
    }

    private void ApplyCommands(
        List<string> commands)
    {
        /*
         * Command1 is bottom.
         * Command2 is middle.
         * Command3 is top.
         *
         * For now Sonic/Ars only uses Command1.
         * Later:
         * commands[0] = Kairi/Riku/Ars slot
         * commands[1] = Cloud slot
         * commands[2] = Ultimate slot
         */
        ApplyCommandSlot(
            _command1,
            _command1Label,
            commands.Count > 0 ? commands[0] : string.Empty,
            ref _lastCommand1Text,
            ref _command1Tween);

        ApplyCommandSlot(
            _command2,
            _command2Label,
            commands.Count > 1 ? commands[1] : string.Empty,
            ref _lastCommand2Text,
            ref _command2Tween);

        ApplyCommandSlot(
            _command3,
            _command3Label,
            commands.Count > 2 ? commands[2] : string.Empty,
            ref _lastCommand3Text,
            ref _command3Tween);
    }

    private void ApplyCommandSlot(
        TextureRect? command,
        RichTextLabel? label,
        string text,
        ref string lastText,
        ref Tween? tween)
    {
        if (command == null ||
            label == null)
        {
            return;
        }

        bool shouldShow =
            !string.IsNullOrEmpty(text);

        if (!shouldShow)
        {
            if (command.Visible &&
                command.Modulate.A > 0.99f)
            {
                SfxCmd.Play(
                    "res://Sora/sfx/formchange.wav");

                PlayDisappearAnimation(
                    command,
                    label,
                    ref tween);
            }

            lastText = string.Empty;
            return;
        }

        bool wasVisible =
            command.Visible;

        bool changed =
            text != lastText;

        label.Text =
            LocalizeCardTitleKey(text);

        command.Visible =
            true;

        if (!wasVisible ||
            changed)
        {
            lastText =
                text;

            PlayAppearAnimation(
                command,
                ref tween);
        }
        else
        {
            lastText =
                text;
        }
    }

    private void PlayAppearAnimation(
        TextureRect command,
        ref Tween? tween)
    {
        tween?.Kill();
        tween = null;

        command.PivotOffset =
            command.Size / 2f;

        command.Scale =
            new Vector2(
                1.8f,
                1.8f);

        SfxCmd.Play(
            "res://Sora/sfx/situation_command.wav");

        tween =
            CreateTween();

        tween.TweenProperty(
                command,
                "scale",
                Vector2.One,
                0.30f)
            .SetTrans(
                Tween.TransitionType.Back)
            .SetEase(
                Tween.EaseType.Out);
    }

    private void HideAllCommands()
    {
        HideCommand(
            _command1,
            _command1Label,
            ref _lastCommand1Text,
            ref _command1Tween);

        HideCommand(
            _command2,
            _command2Label,
            ref _lastCommand2Text,
            ref _command2Tween);

        HideCommand(
            _command3,
            _command3Label,
            ref _lastCommand3Text,
            ref _command3Tween);
    }

    private static void HideCommand(
        TextureRect? command,
        RichTextLabel? label,
        ref string lastText,
        ref Tween? tween)
    {
        if (command != null)
            command.Visible = false;

        if (label != null)
            label.Text = string.Empty;

        lastText = string.Empty;

        tween?.Kill();
        tween = null;
    }
    
    private void PlayDisappearAnimation(
        TextureRect command,
        RichTextLabel? label,
        ref Tween? tween)
    {
        tween?.Kill();

        command.PivotOffset =
            command.Size / 2f;

        command.Scale = Vector2.One;
        command.Modulate =
            new Color(
                1f,
                1f,
                1f,
                0.99f);

        tween = CreateTween();

        tween.Parallel()
            .TweenProperty(
                command,
                "scale",
                new Vector2(1.3f, 1.3f),
                0.4f)
            .SetTrans(
                Tween.TransitionType.Back)
            .SetEase(
                Tween.EaseType.Out);

        tween.Parallel()
            .TweenProperty(
                command,
                "modulate:a",
                0f,
                0.4f);

        tween.TweenCallback(
            Callable.From(() =>
            {
                command.Visible = false;

                command.Scale =
                    Vector2.One;

                command.Modulate =
                    Colors.White;

                if (label != null)
                    label.Text = string.Empty;
            }));
    }

    private static string LocalizeCardTitleKey(
        string titleKey)
    {
        if (string.IsNullOrEmpty(titleKey))
            return string.Empty;

        var locString =
            new LocString(
                "cards",
                titleKey);

        return LocManager.Instance.SmartFormat(
            locString,
            new Dictionary<string, object>());
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;

        _command1Tween?.Kill();
        _command2Tween?.Kill();
        _command3Tween?.Kill();

        _command1Tween = null;
        _command2Tween = null;
        _command3Tween = null;

        _display = null;

        _command1 = null;
        _command2 = null;
        _command3 = null;

        _command1Label = null;
        _command2Label = null;
        _command3Label = null;

        _player = null;
    }
}

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
public static class SituationCommandDisplayPatch
{
    public static void Postfix(
        NEnergyCounter __instance)
    {
        if (__instance == null)
            return;

        if (!GodotObject.IsInstanceValid(__instance))
            return;

        var state =
            CombatManager.Instance?.DebugOnlyGetState();

        var player =
            state?.Players.FirstOrDefault(
                p => LocalContext.IsMe(p));

        if (player?.Character is not Character.Sora)
            return;

        if (__instance.GetNodeOrNull<SituationCommandDisplay>(
                "SituationCommandDisplay") != null)
        {
            return;
        }

        __instance.AddChild(
            new SituationCommandDisplay
            {
                Name = "SituationCommandDisplay"
            });
    }
}