using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Mechanics.SituationCommand;

public partial class SituationGaugeDisplay : Control
{
    public static SituationGaugeDisplay? Instance { get; private set; }

    private Control? _display;

    private TextureRect? _bar1;
    private TextureRect? _bar2;
    private TextureRect? _bar3;

    private Player? _player;

    private int _lastProgress = -1;
    private int _previousProgress = -1;
    
    private int _lastBar1Amount = -1;
    private int _lastBar2Amount = -1;
    private int _lastBar3Amount = -1;
    
    private Tween? _bar1Tween;
    private Tween? _bar2Tween;
    private Tween? _bar3Tween;
    
    private IHoverTip? _hoverTip;

    public override void _Ready()
    {
        Instance = this;

        Name = "SituationGaugeDisplay";

        MouseFilter = MouseFilterEnum.Pass;

        CallDeferred(nameof(Setup));
    }

    private async void Setup()
    {
        if (!IsInsideTree())
            return;

        for (int i = 0; i < 60; i++)
        {
            var state = CombatManager.Instance?.DebugOnlyGetState();

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
                "res://Sora/scenes/situation_display.tscn");

        if (scene == null)
        {
            GD.PushError(
                "[Sora Situation] Failed to load situation_display.tscn");

            QueueFree();
            return;
        }

        _display = scene.Instantiate<Control>();
        
        _hoverTip = SoraStaticHoverTips.SP;

        _display.Connect(
            SignalName.MouseEntered,
            Callable.From(OnHovered));

        _display.Connect(
            SignalName.MouseExited,
            Callable.From(OnUnhovered));

        AddChild(_display);

        _display.MouseFilter = MouseFilterEnum.Pass;

        _display.SetAnchorsPreset(
            LayoutPreset.BottomLeft);

        // tweak later
        _display.Position =
            new Vector2(-115, -60);

        _bar1 =
            _display.GetNodeOrNull<TextureRect>(
                "bar1");

        _bar2 =
            _display.GetNodeOrNull<TextureRect>(
                "bar2");

        _bar3 =
            _display.GetNodeOrNull<TextureRect>(
                "bar3");

        UpdateDisplay();
    }

    public override void _Process(double delta)
    {
        UpdateDisplay();
    }
    
    private void OnHovered()
    {
        if (_hoverTip == null)
            return;

        NHoverTipSet.Clear();

        var tip =
            NHoverTipSet.CreateAndShow(
                this,
                _hoverTip);

        if (tip != null)
        {
            tip.GlobalPosition =
                GlobalPosition +
                new Vector2(-75f, -450f);

            tip.MouseFilter =
                MouseFilterEnum.Ignore;
        }
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);
    }

    private void UpdateDisplay()
    {
        if (_player == null)
            return;

        if (_bar1 == null ||
            _bar2 == null ||
            _bar3 == null)
            return;

        var relic =
            _player.Relics
                .OfType<SituationRelicBase>()
                .FirstOrDefault();

        if (relic == null)
            return;

        int progress =
            relic.GetArrowProgressForUI();

        if (progress == _lastProgress)
            return;

        _lastProgress = progress;

        SetArrow(
            _bar1,
            progress,
            ref _lastBar1Amount,
            ref _bar1Tween);

        SetArrow(
            _bar2,
            progress - 10,
            ref _lastBar2Amount,
            ref _bar2Tween);

        SetArrow(
            _bar3,
            progress - 20,
            ref _lastBar3Amount,
            ref _bar3Tween);
    }

    private void SetArrow(
        TextureRect arrow,
        int amount,
        ref int lastAmount,
        ref Tween? tween)
    {
        amount =
            Mathf.Clamp(
                amount,
                0,
                10);

        float targetScale =
            GetArrowScale(amount);

        bool increased =
            lastAmount >= 0 &&
            amount > lastAmount;

        bool changed =
            amount != lastAmount;

        lastAmount = amount;

        if (!changed)
            return;

        tween?.Kill();
        tween = null;

        if (!increased)
        {
            arrow.Scale =
                new Vector2(
                    targetScale,
                    targetScale);

            return;
        }

        PulseArrow(
            arrow,
            targetScale,
            ref tween);
    }
    
    private void PulseArrow(
        TextureRect arrow,
        float targetScale,
        ref Tween? tween)
    {
        float pulseScale =
            targetScale + 0.35f;

        arrow.Scale =
            new Vector2(
                targetScale,
                targetScale);

        tween =
            CreateTween();

        tween.TweenProperty(
                arrow,
                "scale",
                new Vector2(
                    pulseScale,
                    pulseScale),
                0.04f)
            .SetEase(
                Tween.EaseType.Out);

        tween.TweenProperty(
                arrow,
                "scale",
                new Vector2(
                    targetScale,
                    targetScale),
                0.14f)
            .SetTrans(
                Tween.TransitionType.Back)
            .SetEase(
                Tween.EaseType.Out);
    }

    private float GetArrowScale(int amount)
    {
        amount =
            Mathf.Clamp(
                amount,
                0,
                10);

        if (amount <= 0)
            return 0f;

        /*
         * 1 SP  = 0.55
         * 10 SP = 1.00
         */
        return Mathf.Lerp(
            0.55f,
            1f,
            (amount - 1) / 9f);
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;

        _display = null;

        _bar1 = null;
        _bar2 = null;
        _bar3 = null;

        _player = null;
        _bar1Tween?.Kill();
        _bar2Tween?.Kill();
        _bar3Tween?.Kill();

        _bar1Tween = null;
        _bar2Tween = null;
        _bar3Tween = null;
        
        NHoverTipSet.Remove(this);
        _hoverTip = null;
    }
}

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
public static class SituationGaugeDisplayOverlayPatch
{
    public static void Postfix(
        NEnergyCounter __instance)
    {
        if (__instance == null)
            return;

        if (!GodotObject.IsInstanceValid(__instance))
            return;
        
        var state = CombatManager.Instance?.DebugOnlyGetState();

        var player = state?.Players.FirstOrDefault(

        p => LocalContext.IsMe(p));
        
        if (player?.Character is not Character.Sora)
            return;

        if (__instance.GetNodeOrNull<SituationGaugeDisplay>(
                "SituationGaugeDisplay") != null)
        {
            return;
        }

        __instance.AddChild(
            new SituationGaugeDisplay
            {
                Name = "SituationGaugeDisplay"
            });
    }
}