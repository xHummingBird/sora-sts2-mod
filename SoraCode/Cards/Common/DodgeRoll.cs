using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Cards.Common;

public class DodgeRoll() : SoraCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(6, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(4m);
    }
}