using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class UltimateCombo() : SoraCard(1, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int BaseTurns = 2;
    private const int ExtendTurns = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<UltimateFormPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();
            sora.PlayAnimation(ownerCreature, "attack");
            await Task.Delay((int)(0.2f * 1000f));
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
            .Execute(choiceContext);

        var creature = base.Owner.Creature;
        int turns = creature.HasPower<UltimateFormPower>() ? ExtendTurns : BaseTurns;

        await PowerCmd.Apply<UltimateFormPower>(
            choiceContext,
            creature,
            turns,
            creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
