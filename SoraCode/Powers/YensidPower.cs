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

// At the start of your turn, upgrade a random card in your hand.
public class YensidPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != base.Owner.Player)
            return;

        CardModel? card =
            PileType.Hand.GetPile(base.Owner.Player).Cards
                .ToList()
                .StableShuffle(base.Owner.Player.RunState.Rng.Shuffle)
                .FirstOrDefault();

        if (card == null)
            return;

        Flash();

        await CardCmd.Upgrade(card, default);
    }
}
