using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class Guardbreak() : SoraCard(2, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(18, ValueProp.Move),
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
        decimal finalDamage = base.DynamicVars.Damage.BaseValue;
        if (play.Target.HasPower<VulnerablePower>())
            finalDamage = 1.5m * base.DynamicVars.Damage.BaseValue;
        
        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            string attackVfx = base.Owner.Creature.HasPower<UltimateFormPower>()
                ? "hit_ultimate"
                : "atk_vfx";
            
            
            await sora.DashTo(ownerCreature, play.Target, distance: 300f);
            AudioHelper.PlayRandomFinalAttack2();
            if (!ownerCreature.HasPower<UltimateFormPower>())
            {
                sora.PlayAnimation(ownerCreature, "attack");

                await Task.Delay((int)(0.2f * 1000f));
                sora.DashPast(base.Owner.Creature, play.Target, null, 200f);
                SfxCmd.Play("res://Sora/sfx/swing_down.wav");
                
                sora.PlayVfxOnTarget(
                    play.Target,
                    "res://Sora/scenes/vfx.tscn",
                    attackVfx
                );
                
                await DamageCmd.Attack(finalDamage).FromCard(this, play)
                    .Targeting(play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
                    .Execute(choiceContext);
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
                await DamageCmd.Attack(finalDamage).FromCard(this, play)
                    .Targeting(play.Target)
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
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}