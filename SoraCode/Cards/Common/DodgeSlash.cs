using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Common;

public class DodgeSlash() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        string hitSfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "res://Sora/sfx/ultimate_hit_1.wav"
            : "res://Sora/sfx/hit_medium.wav";
        
        string attackAnim = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "attack_ultimate"
            : "dodge_slash";
        
        string attackVfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "hit_ultimate"
            : "atk_vfx";

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();

            sora.PlayAnimation(ownerCreature, attackAnim);

            await Task.Delay((int)(0.2f * 1000f));

            SfxCmd.Play("res://Sora/sfx/swing_down.wav");

            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/vfx.tscn",
                attackVfx
            );
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", hitSfx)
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}