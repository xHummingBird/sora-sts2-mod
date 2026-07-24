using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Sora.SoraCode.Character;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Extensions;

public class SoraExtensions
{
    public enum SwingSfx
    {
        SwingUp,
        SwingDown
    }

    public enum HitSfx
    {
        HitUp,
        HitDown
    }
    
    
    public static class CombatHelpers
    {
        private const string DefaultVfx = "vfx/vfx_attack_slash";

        private static string GetSwingSfx(SwingSfx? swing) =>
            swing switch
            {
                SwingSfx.SwingUp => "res://Sora/sfx/swing_up.wav",
                SwingSfx.SwingDown => "res://Sora/sfx/swing_down.wav",
                _ => "res://Sora/sfx/swing_up.wav"
            };

        private static string GetHitSfx(HitSfx? hit) =>
            hit switch
            {
                HitSfx.HitUp => "res://Sora/sfx/hit_up.wav",
                HitSfx.HitDown => "res://Sora/sfx/hit_down.wav",
                _ => "res://Sora/sfx/hit_up.wav"
            };

        public static async Task FakeHit(
            Creature target,
            SwingSfx? swing = SwingSfx.SwingUp,
            HitSfx? hit = HitSfx.HitUp,
            string? swingOverride = null,
            string? hitOverride = null)
        {
            if (target == null)
                return;

            SfxCmd.Play(swingOverride ?? GetSwingSfx(swing));

            VfxCmd.PlayOnCreatureCenter(target, DefaultVfx);

            SfxCmd.Play(hitOverride ?? GetHitSfx(hit));

            await CreatureCmd.TriggerAnim(target, "Hit", 0f);

            if (target.Monster?.HasHurtSfx == true)
            {
                SfxCmd.Play(target.Monster.HurtSfx);
            }
        }
        public static async Task RefreshLink<T>(
            PlayerChoiceContext choiceContext,
            Creature target,
            CardModel source,
            int turns = 3)
            where T : PowerModel
        {
            int amountToAdd =
                Math.Max(0, turns - target.GetPowerAmount<T>());

            if (amountToAdd > 0)
            {
                await PowerCmd.Apply<T>(
                    choiceContext,
                    target,
                    amountToAdd,
                    target,
                    source);
            }
        }
        
        public static int CountActiveLinks(Creature creature)
        {
            int count = 0;
            
            if (creature.HasPower<RikuPower>())
                count++;
            
            if (creature.HasPower<KairiPower>())
                count++;
            
            if (creature.HasPower<CloudPower>())
                count++;
            
            if (creature.HasPower<MickeyPower>())
                count++;
            
            if (creature.HasPower<YensidPower>())
                count++;
            
            return count;
        }
    }
    
    async Task ComboHit(
        Character.Sora sora,
        Creature target,
        float delaySeconds,
        bool playSound = false)
    {
        SoraExtensions.CombatHelpers.FakeHit(target, null, null);
        sora.PlayVfxOnTarget(target, "res://Sora/scenes/vfx.tscn", "atk_vfx");

        if (playSound)
            AudioHelper.PlayRandomAttack();

        if (delaySeconds > 0)
            await Task.Delay((int)(delaySeconds * 1000f));
    }
}