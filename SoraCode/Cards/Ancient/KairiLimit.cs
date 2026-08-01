using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.SituationCommand;

namespace Sora.SoraCode.Cards.Ancient;


public class KairiLimit() : SoraCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(30, ValueProp.Move),
        new HealVar(5)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            AudioHelper.PlayRandomKairi();
            await sora.DashTo(ownerCreature, play.Target, distance: 390f);
            float duration = sora.PlayAnimation(ownerCreature, "seven_wishes").total;
            SfxCmd.Play("res://Sora/sounds/formchange_2.wav");
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_finish_3.wav");
            
            await Task.Delay((int)(0.2f * 1000f));
            SfxCmd.Play("res://Sora/sfx/formchange_3.wav");
            
            await Task.Delay((int)(1.133f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomKairiAtk();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_up.wav", hitOverride: "res://Sora/sfx/hit_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (1).wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.667f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomKairiAtk();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_up.wav", hitOverride: "res://Sora/sfx/hit_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (2).wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.533f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomKairiAtk();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_up.wav", hitOverride: "res://Sora/sfx/hit_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (3).wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.467f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomKairiAtk();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_up.wav", hitOverride: "res://Sora/sfx/hit_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (4).wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.467f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomKairiAtk();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_up.wav", hitOverride: "res://Sora/sfx/hit_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (5).wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.467f * 1000f));
            
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_up.wav", hitOverride: "res://Sora/sfx/hit_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (6).wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.2f * 1000f));
            SfxCmd.Play("res://Sora/sounds/formchange_2.wav");
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_finish_3.wav");
            
            await Task.Delay((int)(0.233f * 1000f));
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (1).wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_1.wav", hitOverride:"res://Sora/sfx/hit_medium.wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (2).wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/swing_1.wav", hitOverride:"res://Sora/sfx/ars_hit2_4.wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            
            await Task.Delay((int)(0.4f * 1000f));
            SfxCmd.Play("res://Sora/sounds/finalhit_9.wav");
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_finish_2.wav");
            SfxCmd.Play("res://Sora/sfx/formchange.wav");
            SfxCmd.Play("res://Sora/sfx/ars_finalhit.wav");
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Sora/sfx/kairi/kairi_keyblade_hit (7).wav");
            sora.DoScreenShake(ShakeStrength.Strong);
            await CommonActions.CardAttack(this, play.Target)
                .Execute(choiceContext);
            await Task.Delay((int)(0.8f * 1000f));
            await sora.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  
            await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
            .Execute(choiceContext);
        await CreatureCmd.Heal(base.Owner.Creature, DynamicVars.Heal.BaseValue);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
        DynamicVars.Heal.UpgradeValueBy(2);
    }
}