using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sora.SoraCode.Mechanics.Companion;

namespace Sora.SoraCode.Powers;

// Each time you play a Companion card, draw a card.
public class BondOfLIghtPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
            return;

        if (cardPlay.Card is not ICompanionCard)
            return;

        Flash();

        await CardPileCmd.Draw(choiceContext, 1, base.Owner.Player);
    }
}
