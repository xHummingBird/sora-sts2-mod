using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Sora.SoraCode.Powers;

// At the start of your turn, return a random attack card from your discard
// pile to your hand.
public class PowerOfWakingPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != base.Owner.Player)
            return;

        CardModel? card =
            PileType.Discard.GetPile(base.Owner.Player).Cards
                .Where(c => c.Type == CardType.Attack)
                .ToList()
                .StableShuffle(base.Owner.Player.RunState.Rng.Shuffle)
                .FirstOrDefault();

        if (card == null)
            return;

        Flash();

        await CardPileCmd.Add(card, PileType.Hand);
    }
}
