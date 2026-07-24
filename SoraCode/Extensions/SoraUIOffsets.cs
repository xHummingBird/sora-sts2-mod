// using System.Runtime.CompilerServices;
// using Godot;
// using HarmonyLib;
// using MegaCrit.Sts2.addons.mega_text;
// using MegaCrit.Sts2.Core.Combat;
// using MegaCrit.Sts2.Core.Context;
// using MegaCrit.Sts2.Core.Nodes.Cards;
// using MegaCrit.Sts2.Core.Nodes.Combat;
// using Sora.SoraCode.Character;
//
// namespace Sora.SoraCode.Extensions;
//
// public static class SoraUiOffsets
// {
//     public static readonly ConditionalWeakTable<MegaLabel, LabelPositionData>
//         Positions = new();
//
//     public class LabelPositionData
//     {
//         public Vector2 OriginalPosition;
//     }
// }
//
// [HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
// public static class SoraCardEnergyIconPatch
// {
//     static readonly AccessTools.FieldRef<NCard, TextureRect>
//         EnergyIconRef =
//             AccessTools.FieldRefAccess<NCard, TextureRect>("_energyIcon");
//
//     static void Postfix(NCard __instance)
//     {
//         if (__instance.Model?.Pool is not SoraCardPool)
//             return;
//
//         var energyIcon = EnergyIconRef(__instance);
//
//         energyIcon.Position += Vector2.Up * 3;
//     }
// }
//
// [HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
// public static class SoraEnergyCounterPatch
// {
//     static readonly AccessTools.FieldRef<NEnergyCounter, Control>
//         LayersRef =
//             AccessTools.FieldRefAccess<NEnergyCounter, Control>("_layers");
//
//     static readonly AccessTools.FieldRef<NEnergyCounter, Control>
//         RotationLayersRef =
//             AccessTools.FieldRefAccess<NEnergyCounter, Control>("_rotationLayers");
//
//     static void Postfix(NEnergyCounter __instance)
//     {
//         var state =
//             CombatManager.Instance?.DebugOnlyGetState();
//
//         var player =
//             state?.Players.FirstOrDefault(
//                 p => LocalContext.IsMe(p));
//
//         if (player?.Character is not Character.Sora)
//             return;
//
//         LayersRef(__instance).Position += Vector2.Up * 3;
//         RotationLayersRef(__instance).Position += Vector2.Up * 3;
//     }
// }