using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class FinishBoost() : SoraCard(2, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 50 stacks = +50% damage on the third attack each turn.
        new PowerVar<FinishBoostPower>(50),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FinishBoostPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<FinishBoostPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["FinishBoostPower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FinishBoostPower"].UpgradeValueBy(25m);
    }
}
