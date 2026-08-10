using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Basic;

public class FullCombo() : SoraCard(2, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DamageVar(4, ValueProp.Move),
            new RepeatVar(3)
        ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            decimal damage = base.DynamicVars.Damage.PreviewValue;
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await sora.DashTo(ownerCreature, play.Target, distance: 350f);
            AudioHelper.PlayRandomAttack();
            if (!ownerCreature.HasPower<UltimateFormPower>())
            {
                float duration = sora.PlayAnimation(ownerCreature, "full_combo").total;

                await Task.Delay((int)(0.13f * 1000f));
                DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                    .WithValueProp(ValueProp.Unpowered)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/swing_down.wav")
                    .Execute(choiceContext);
                SfxCmd.Play("res://Sora/sfx/hit_down.wav");
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
                await Task.Delay((int)(0.266f * 1000f));
                SoraExtensions.CombatHelpers.FakeHit(play.Target);
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
                await Task.Delay((int)(0.3333f * 1000f));
                DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                    .WithValueProp(ValueProp.Unpowered)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/swing_down.wav")
                    .Execute(choiceContext);
                SfxCmd.Play("res://Sora/sfx/hit_medium.wav");
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
                await Task.Delay((int)(0.4f * 1000f));
                AudioHelper.PlayRandomFinalAttack();
                await Task.Delay((int)(0.467f * 1000f));
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
                await CommonActions.CardAttack(this, play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
                    .Execute(choiceContext);
                await Task.Delay((int)(0.36f * 1000f));
                await sora.Retreat(ownerCreature);
            }
            else
            {
                sora.PlayAnimation(ownerCreature, "ultimate_combo");
                await Task.Delay((int)(0.133f * 1000f));
                DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                    .WithValueProp(ValueProp.Unpowered)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/ultimate_swing_1.wav")
                    .Execute(choiceContext);
                SfxCmd.Play("res://Sora/sfx/ultimate_hit_1.wav");
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "hit_ultimate");
                await Task.Delay((int)(0.266f * 1000f));
                DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                    .WithValueProp(ValueProp.Unpowered)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/ultimate_swing_2.wav")
                    .Execute(choiceContext);
                SfxCmd.Play("res://Sora/sfx/ultimate_hit_2.wav");
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "hit_ultimate");
                await Task.Delay((int)(0.233f * 1000f));
                AudioHelper.PlayRandomFinalAttack();
                await Task.Delay((int)(0.3f * 1000f));
                sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "hit_ultimate");
                SfxCmd.Play("res://Sora/sfx/ultimate_thrust.wav");
                await CommonActions.CardAttack(this, play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/ultimate_hit_3.wav")
                    .Execute(choiceContext);
                await Task.Delay((int)(0.7f * 1000f));
                await sora.Retreat(ownerCreature, null, true, 0.01f);
                await Task.Delay((int)(0.133f * 1000f));
            }

            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  
            await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
            .Execute(choiceContext);
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints(4);
        }
        
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}