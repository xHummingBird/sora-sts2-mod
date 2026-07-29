using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using Sora.SoraCode.Mechanics.SituationCommand;

namespace Sora.SoraCode.Extensions;

/*
 * The energy number on a card and the "1/3" on the energy counter are
 * positioned by the game's own scenes, so the only way to nudge them is to
 * grab the label nodes after the game builds them and move them ourselves.
 *
 * Nothing here relies on hardcoded private field names: the label nodes are
 * found by looking for a Control that exposes a string "text" property, which
 * covers Label, RichTextLabel and MegaLabel alike. If the game renames or
 * reshuffles its nodes, flip DebugDumpTargets to see what is actually there.
 */
public static class SoraUiOffsets
{
    /*
     * How far down to nudge the text, in pixels.
     * Positive = down, negative = up.
     */
    public const float CardEnergyCostDown = 2f;

    public const float EnergyCounterTextDown = 3f;

    /*
     * Set to true to print the node trees these offsets are applied to.
     * Useful when a nudge lands on the wrong node.
     */
    private const bool DebugDumpTargets = false;

    private static readonly StringName TextProperty = "text";

    private static readonly string[] EnergyNameHints =
    [
        "energy",
        "cost"
    ];

    /*
     * Keyed on the label so re-running a patch (Godot can re-run _Ready via
     * RequestReady) re-applies the same offset instead of stacking a new one.
     */
    private static readonly ConditionalWeakTable<Control, OffsetState> Offsets = new();

    private static readonly HashSet<string> WarnedKeys = [];

    private sealed class OffsetState
    {
        public Vector2 BasePosition;
    }

    public static void PushDown(
        Control? control,
        float pixels)
    {
        if (control == null)
            return;

        if (!GodotObject.IsInstanceValid(control))
            return;

        var state =
            Offsets.GetValue(
                control,
                c => new OffsetState
                {
                    BasePosition = c.Position
                });

        control.Position =
            state.BasePosition +
            new Vector2(0f, pixels);
    }

    public static void OffsetCardEnergyCost(NCard? card)
    {
        if (card == null)
            return;

        if (!GodotObject.IsInstanceValid(card))
            return;

        /*
         * The card frame is shared with the base game, so only touch it while
         * the local player is actually running Sora.
         */
        if (!SoraRunContext.IsSoraRun)
            return;

        var energyRoot = FindEnergyNode(card);

        if (energyRoot == null)
        {
            WarnOnce(
                "card-energy",
                "[Sora UI] Could not find the energy cost node on NCard; the card cost offset was skipped.");

            return;
        }

        if (DebugDumpTargets)
            DumpTree(energyRoot, $"NCard energy cost root '{energyRoot.Name}'");

        // The badge art stays put; only the number moves.
        if (IsTextNode(energyRoot))
        {
            PushDown(energyRoot, CardEnergyCostDown);
            return;
        }

        var labels = FindTextNodes(energyRoot);

        if (labels.Count > 0)
        {
            foreach (var label in labels)
                PushDown(label, CardEnergyCostDown);

            return;
        }

        // No separate number node, so nudge the whole badge instead.
        PushDown(energyRoot, CardEnergyCostDown);
    }

    public static void OffsetEnergyCounterText(NEnergyCounter? counter)
    {
        if (counter == null)
            return;

        if (!GodotObject.IsInstanceValid(counter))
            return;

        if (DebugDumpTargets)
            DumpTree(counter, "NEnergyCounter");

        var labels = FindTextNodes(counter);

        if (labels.Count == 0)
        {
            WarnOnce(
                "energy-counter",
                "[Sora UI] Found no text nodes under NEnergyCounter; the energy counter offset was skipped.");

            return;
        }

        foreach (var label in labels)
            PushDown(label, EnergyCounterTextDown);
    }

    /*
     * Runs the offset one frame later, once the game has finished its own
     * setup and the first layout pass has settled.
     */
    public static void DeferOffset(Action action)
    {
        Callable.From(action).CallDeferred();
    }

    private static Control? FindEnergyNode(Node instance)
    {
        Control? fallback = null;

        foreach (var field in InstanceFields(instance.GetType()))
        {
            if (!typeof(Control).IsAssignableFrom(field.FieldType))
                continue;

            if (!MatchesEnergyHint(field.Name))
                continue;

            if (field.GetValue(instance) is not Control control)
                continue;

            if (!GodotObject.IsInstanceValid(control))
                continue;

            if (IsTextNode(control))
                return control;

            fallback ??= control;
        }

        return fallback ?? FindNodeByEnergyHint(instance);
    }

    private static IEnumerable<FieldInfo> InstanceFields(Type? type)
    {
        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        while (type != null && type != typeof(object))
        {
            foreach (var field in type.GetFields(flags))
                yield return field;

            type = type.BaseType;
        }
    }

    private static Control? FindNodeByEnergyHint(Node root)
    {
        foreach (var child in root.GetChildren())
        {
            if (IsOwnNode(child))
                continue;

            if (child is Control control && MatchesEnergyHint(child.Name.ToString()))
                return control;

            var nested = FindNodeByEnergyHint(child);

            if (nested != null)
                return nested;
        }

        return null;
    }

    private static List<Control> FindTextNodes(Node root)
    {
        List<Control> found = [];

        CollectTextNodes(root, found);

        return found;
    }

    private static void CollectTextNodes(
        Node root,
        List<Control> found)
    {
        foreach (var child in root.GetChildren())
        {
            if (IsOwnNode(child))
                continue;

            /*
             * Do not recurse into a label: outlined text is often built from
             * duplicated child labels, and those already move with the parent.
             */
            if (child is Control control && IsTextNode(child))
            {
                found.Add(control);
                continue;
            }

            CollectTextNodes(child, found);
        }
    }

    /*
     * Our own overlays are parented to NEnergyCounter and carry labels of
     * their own, so they must never be swept up by the search.
     */
    private static bool IsOwnNode(Node node)
    {
        return node is SituationGaugeDisplay or SituationCommandDisplay;
    }

    private static bool IsTextNode(Node node)
    {
        if (node is Label or RichTextLabel)
            return true;

        // Covers MegaLabel and anything else exposing a "text" property.
        return node is Control &&
               node.Get(TextProperty).VariantType == Variant.Type.String;
    }

    private static bool MatchesEnergyHint(string name)
    {
        string lowered = name.ToLowerInvariant();

        return EnergyNameHints.Any(hint => lowered.Contains(hint));
    }

    private static void DumpTree(
        Node root,
        string tag,
        int depth = 0)
    {
        if (depth == 0)
            GD.Print($"[Sora UI] ---- {tag} ----");

        GD.Print(
            $"[Sora UI] {new string(' ', depth * 2)}{root.Name} ({root.GetType().Name})" +
            (root is Control c ? $" pos={c.Position}" : string.Empty) +
            (IsTextNode(root) ? $" text=\"{root.Get(TextProperty).AsString()}\"" : string.Empty));

        foreach (var child in root.GetChildren())
            DumpTree(child, tag, depth + 1);
    }

    private static void WarnOnce(
        string key,
        string message)
    {
        if (!WarnedKeys.Add(key))
            return;

        GD.PushWarning(message);
    }
}

/*
 * Cards use the same node layout for every character, and NCard._Ready runs
 * before the card model is bound, so "is this a Sora card" is not answerable
 * at patch time. Tracking the run's character instead keeps the offset out of
 * other characters' runs.
 */
public static class SoraRunContext
{
    public static bool IsSoraRun { get; private set; }

    public static void Update(IRunState? runState)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player == null)
            return;

        IsSoraRun = player.Character is Character.Sora;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterActEntered))]
public static class SoraRunContextActPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState)
    {
        SoraRunContext.Update(runState);
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
public static class SoraRunContextRoomPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState)
    {
        SoraRunContext.Update(runState);
    }
}

[HarmonyPatch]
public static class SoraCardEnergyOffsetPatch
{
    /*
     * _Ready is the hook we want, but it is only patchable if NCard actually
     * declares it. Resolving it here (instead of via a [HarmonyPatch] attribute)
     * means a rename in the game cannot take Harmony.PatchAll down with it.
     */
    private static readonly string[] HookCandidates =
    [
        "_Ready",
        "SetCard",
        "SetModel",
        "UpdateCard",
        "Refresh"
    ];

    private static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (string name in HookCandidates)
        {
            var method = AccessTools.DeclaredMethod(typeof(NCard), name);

            if (method == null)
                continue;

            yield return method;

            yield break;
        }

        GD.PushWarning(
            "[Sora UI] NCard exposes none of the expected hooks; the card energy cost offset is inactive.");
    }

    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        if (__instance == null)
            return;

        if (!GodotObject.IsInstanceValid(__instance))
            return;

        SoraUiOffsets.DeferOffset(
            () => SoraUiOffsets.OffsetCardEnergyCost(__instance));
    }
}

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
public static class SoraEnergyCounterOffsetPatch
{
    [HarmonyPostfix]
    public static void Postfix(NEnergyCounter __instance)
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

        SoraUiOffsets.DeferOffset(
            () => SoraUiOffsets.OffsetEnergyCounterText(__instance));
    }
}
