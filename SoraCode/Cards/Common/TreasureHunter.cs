using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Common;

public class TreasureHunter() : SoraCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DynamicVar("Sp", 2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner);

        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
        relic?.GainSituationPoints((int)DynamicVars["Sp"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["Sp"].UpgradeValueBy(1m);
    }
}
