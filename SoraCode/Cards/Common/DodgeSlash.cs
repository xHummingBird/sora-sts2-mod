using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;

namespace Sora.SoraCode.Cards.Common;

public class DodgeSlash() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new BlockVar(5, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();

            sora.PlayAnimation(ownerCreature, "dodge_slash");

            await Task.Delay((int)(0.2f * 1000f));

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
        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}