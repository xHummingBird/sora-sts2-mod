using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Ancient;

public class UltimateCombo() : SoraCard(1, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy)
{
    private const int BaseTurns = 2;
    private const int ExtendTurns = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13m, ValueProp.Move),
        new DynamicVar("Turns", 2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<UltimateFormPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        bool hasUltimate = ownerCreature.HasPower<UltimateFormPower>();
        
        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            if (!hasUltimate)
            {
                AudioHelper.PlayRandomFormchange();
                sora.PlayAnimation(ownerCreature, "ultimate_form");
                SfxCmd.Play("res://Sora/sfx/ultimate_form.mp3");
                PowerCmd.Apply<UltimateFormPower>(
                    choiceContext,
                    ownerCreature,
                    DynamicVars["Turns"].BaseValue,
                    ownerCreature,
                    this);
                await Task.Delay(2265);
                sora.PlayAnimation(ownerCreature, "idle_ultimate");
            }
            else await PowerCmd.Apply<UltimateFormPower>(
                choiceContext,
                ownerCreature,
                DynamicVars["Turns"].BaseValue - 1,
                ownerCreature,
                this);
            await sora.DashTo(ownerCreature, play.Target, distance: 350f);
            AudioHelper.PlayRandomAttack();
            sora.PlayAnimation(ownerCreature, "ultimate_combo");
            await Task.Delay((int)(0.133f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/ultimate_swing_1.wav", hitOverride: "res://Sora/sfx/ultimate_hit_1.wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "hit_ultimate");
            await Task.Delay((int)(0.266f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, swingOverride: "res://Sora/sfx/ultimate_swing_2.wav", hitOverride: "res://Sora/sfx/ultimate_hit_2.wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "hit_ultimate");
            await Task.Delay((int)(0.233f * 1000f));
            AudioHelper.PlayRandomFinalAttack();
            await Task.Delay((int)(0.3f * 1000f));
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "hit_ultimate");
            SfxCmd.Play("res://Sora/sfx/ultimate_thrust.wav");
            CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/ultimate_hit_3.wav")
                .Execute(choiceContext);
            await Task.Delay((int)(0.7f * 1000f));
            await sora.Retreat(ownerCreature, null, true, 0.01f);
            await Task.Delay((int)(0.133f * 1000f));
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
        {
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
                .Execute(choiceContext);
            await PowerCmd.Apply<UltimateFormPower>(
                choiceContext,
                ownerCreature,
                DynamicVars["Turns"].BaseValue,
                ownerCreature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}