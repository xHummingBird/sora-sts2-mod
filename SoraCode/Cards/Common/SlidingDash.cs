using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Common;

public class SlidingDash() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(9, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        string attackVfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "hit_ultimate"
            : "atk_vfx";

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            
            await sora.DashTo(ownerCreature, play.Target, distance: 300f);
            AudioHelper.PlayRandomAttack();
            if (!ownerCreature.HasPower<UltimateFormPower>())
            {
                sora.PlayAnimation(ownerCreature, "attack");

                await Task.Delay((int)(0.2f * 1000f));
                sora.DashPast(base.Owner.Creature, play.Target, null, 0.19f);
                SfxCmd.Play("res://Sora/sfx/swing_down.wav");

                sora.PlayVfxOnTarget(
                    play.Target,
                    "res://Sora/scenes/vfx.tscn",
                    attackVfx
                );
                await CommonActions.CardAttack(this, play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_medium.wav")
                    .Execute(choiceContext);
                await Task.Delay((int)(0.2f * 1000f));
                sora.Retreat(ownerCreature);
            }

            else
            {
                sora.PlayAnimation(ownerCreature, "attack_ultimate_3");
                await Task.Delay((int)(0.2f * 1000f));
                SfxCmd.Play("res://Sora/sfx/ultimate_thrust.wav");
                sora.PlayVfxOnTarget(
                    play.Target,
                    "res://Sora/scenes/vfx.tscn",
                    attackVfx
                );
                CommonActions.CardAttack(this, play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/ultimate_hit_3.wav")
                    .Execute(choiceContext);
                await Task.Delay((int)(0.81f * 1000f));
                sora.Retreat(ownerCreature, null, true,0.01f);
                await Task.Delay((int)(0.18f * 1000f));
            }
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_medium.wav")
                .Execute(choiceContext);
        base.EnergyCost.SetThisCombat(0);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}