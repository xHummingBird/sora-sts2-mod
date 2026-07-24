using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Rare;

public class KingsGuidance() : SoraCard(2, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MickeyPower>(1),
        new DynamicVar("Sp", 2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MickeyPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        AudioHelper.PlayRandomFormchange();

        await PowerCmd.Apply<MickeyPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["MickeyPower"].BaseValue,
            base.Owner.Creature,
            this);

        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
        relic?.GainSituationPoints((int)DynamicVars["Sp"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
