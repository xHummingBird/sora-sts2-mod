using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Sora.SoraCode.Extensions;

public static class Optimisation
{
    private static PackedScene? _soraScene;
    private static PackedScene? _iceScene;
    private static PackedScene? _vfxScene;
    private static PackedScene? _rikuScene;
    
    private const string SoraScenePath = "res://Sora/scenes/sora.tscn";
    private const string IceVfxPath = "res://Sora/scenes/ice_vfx.tscn";
    private const string VfxPath = "res://Sora/scenes/vfx.tscn";
    private const string RikuScenePath = "res://Sora/scenes/riku.tscn";
    
    public static PackedScene? SoraScene
    {
        get
        {
            _soraScene = LoadOrReload(_soraScene, SoraScenePath, "Sora scene");
            return _soraScene;
        }
    }

    public static PackedScene? IceScene
    {
        get
        {
            _iceScene = LoadOrReload(_iceScene, IceVfxPath, "Ice VFX");
            return _iceScene;
        }
    }

    public static PackedScene? VfxScene
    {
        get
        {
            _vfxScene = LoadOrReload(_vfxScene, VfxPath, "VFX");
            return _vfxScene;
        }
    }
    
    public static PackedScene? RikuScene
    {
        get
        {
            _soraScene = LoadOrReload(_rikuScene, RikuScenePath, "Riku scene");
            return _soraScene;
        }
    }
    
    private static PackedScene? LoadOrReload(PackedScene? cachedScene, string path, string label)
    {
        if (cachedScene != null && GodotObject.IsInstanceValid(cachedScene))
            return cachedScene;

        GD.Print($"SoraAssets: Loading {label} from {path}");

        var scene = GD.Load<PackedScene>(path);

        if (scene == null)
        {
            GD.PrintErr($"SoraAssets: FAILED to load {label}: {path}");
            return null;
        }

        GD.Print($"SoraAssets: Loaded {label}");
        return scene;
    }
    
    public static void EnsurePreloaded()
    {
        _ = SoraScene;
        _ = IceScene;
        _ = VfxScene;
        _ = RikuScene;

        GD.Print("SoraAssets: EnsurePreloaded finished");
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterActEntered))]
public static class SoraAfterActEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player?.Character is not Character.Sora)
            return;

        GD.Print("AfterActEntered: Sora detected → preloading");

        Optimisation.EnsurePreloaded();
    }
}


[HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
public static class SoraAfterRoomEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState, AbstractRoom room)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player?.Character is not Character.Sora)
            return;

        GD.Print($"AfterRoomEntered: Sora detected → preloading. Room = {room.GetType().Name}");

        Optimisation.EnsurePreloaded();
    }
}