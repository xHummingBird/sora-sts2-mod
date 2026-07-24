using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.SituationCommand;

namespace Sora.SoraCode.Cards.Ancient;

public class ArsArcanum() : SoraCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(30, ValueProp.Move)
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
            await sora.DashTo(ownerCreature, play.Target, distance: 390f);
            AudioHelper.PlayRandomFormchange();
            
            float duration = sora.PlayAnimation(ownerCreature, "ars_arcanum").total;
            if (duration > 0f)
                await Task.Delay((int)(1.167f * 1000f));
            AudioHelper.PlayRandomAttack();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            AudioHelper.PlayRandomAttack();
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            AudioHelper.PlayRandomAttack();
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.067f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.233f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.2f * 1000f));
            AudioHelper.PlayRandomAttack();
            await Task.Delay((int)(0.2f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.3f * 1000f));
            SfxCmd.Play("res://Sora/sounds/finalhit_2.wav");
            await Task.Delay((int)(0.1f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.3f * 1000f));
            SfxCmd.Play("res://Sora/sounds/finalhit_6.wav");
            SfxCmd.Play("res://Sora/sfx/formchange.wav");
            SfxCmd.Play("res://Sora/sfx/ars_finalhit.wav");
            await Task.Delay((int)(0.333f * 1000f));
            sora.DoScreenShake(ShakeStrength.Strong);
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await Task.Delay((int)(0.8f * 1000f));
            await sora.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  
            await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
            .Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }
}