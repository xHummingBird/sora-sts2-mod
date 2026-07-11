using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;

namespace Sora.SoraCode.Cards.Basic;

public class SonicStrike() : SoraCard(0, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DamageVar(3, ValueProp.Move),
            new PowerVar<VulnerablePower>(1)
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
            AudioHelper.PlayRandomAttack();
            
            sora.PlayAnimation(ownerCreature, "sonic_strike");
            
            await Task.Delay((int)(0.1f * 1000f));
            
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            
            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/vfx.tscn",
                "atk_vfx"
            );
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_medium.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, base.DynamicVars.Vulnerable.BaseValue,
            base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}