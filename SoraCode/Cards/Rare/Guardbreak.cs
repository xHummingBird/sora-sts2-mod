using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;

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

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomFinalAttack2();
            await sora.DashTo(ownerCreature, play.Target, distance: 300f);
            sora.PlayAnimation(ownerCreature, "attack");
            
            await Task.Delay((int)(0.2f * 1000f));
            sora.DashPast(base.Owner.Creature, play.Target, null, 200f);
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            
            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/vfx.tscn",
                "atk_vfx"
            );
            decimal finalDamage = base.DynamicVars.Damage.BaseValue;
            if (play.Target.HasPower<VulnerablePower>())
                finalDamage = 1.5m * base.DynamicVars.Damage.BaseValue;
            
            await DamageCmd.Attack(finalDamage).FromCard(this, play)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
                .Execute(choiceContext);
            sora.Retreat(ownerCreature);
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