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

public class RikuLimit() : SoraCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(33, ValueProp.Move)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            AudioHelper.PlayRandomRiku();
            await sora.DashTo(ownerCreature, play.Target, distance: 390f);
            SfxCmd.Play("res://Sora/sounds/riku/riku_coop.wav");
            float duration = sora.PlayAnimation(ownerCreature, "dark_arcanum_2").total;
            await Task.Delay((int)(1.167f * 1000f));
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomRikuAtk();
            SfxCmd.Play("res://Sora/sfx/riku/riku_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_hit_1.wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomRikuAtk();
            SfxCmd.Play("res://Sora/sfx/riku/riku_swing_down.wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_hit_hard (3).wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.067f * 1000f));
            AudioHelper.PlayRandomRikuAtk();
            SfxCmd.Play("res://Sora/sfx/riku/riku_swing_up.wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_hit_1.wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            await Task.Delay((int)(0.066f * 1000f));
            AudioHelper.PlayRandomAttack();
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.067f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/riku_swing_up.wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_hit_1.wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.233f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/riku_swing_down.wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_hit_hard (3).wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.2f * 1000f));
            AudioHelper.PlayRandomAttack();
            AudioHelper.PlayRandomRikuAtk();
            await Task.Delay((int)(0.2f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/riku_thrust.wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_hit_1.wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.3f * 1000f));
            SfxCmd.Play("res://Sora/sounds/finalhit_2.wav");
            SfxCmd.Play("res://Sora/sounds/riku/riku_finisher (11).wav");
            SfxCmd.Play("res://Sora/sfx/riku/riku_3_swing.wav");
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
            SfxCmd.Play("res://Sora/sounds/riku/riku_finisher (2).wav");
            SfxCmd.Play("res://Sora/sfx/formchange.wav");
            SfxCmd.Play("res://Sora/sfx/ars_finalhit.wav");
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/dark_firaga.wav");
            sora.DoScreenShake(ShakeStrength.Strong);
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .WithHitVfxSpawnedAtBase()
                .BeforeDamage(async delegate
                {
                    var vfx = NGroundFireVfx.Create(play.Target, VfxColor.Purple);
                        if (vfx != null)
                        {
                            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                            SfxCmd.Play("event:/sfx/characters/attack_fire");
                        }
                })
                .Execute(choiceContext);
            await Task.Delay((int)(0.8f * 1000f));
            await sora.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  
            await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
            .Execute(choiceContext);
        int num = (play.Target.IsAlive ? play.Target.GetPowerAmount<VulnerablePower>() : 0);
        if (num > 0)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, num, base.Owner.Creature, this);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7);
    }
}