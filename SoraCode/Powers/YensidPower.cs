using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Sora.SoraCode.Powers;

// At the start of your turn, upgrade a random card in your hand.
public class YensidPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // public override async Task AfterPlayerTurnStart(
    //     PlayerChoiceContext choiceContext,
    //     Player player)
    // {
    //     if (player != base.Owner.Player)
    //         return;
    //     
    //     var cards =
    //     PileType.Hand.GetPile(base.Owner.Player)
    //         .Cards
    //         .Where(c => c.IsUpgradable)
    //         .TakeRandom(base.Amount, base.Owner.Player.RunState.Rng.CombatCardSelection)
    //         .ToList();
    //     
    //     if (cards.Count == 0)
    //         return;
    //
    //     Flash();
    //
    //     foreach (CardModel item in cards)
    //     {
    //         CardCmd.Upgrade(item);
    //     }
    // }
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        CardPile pile = PileType.Discard.GetPile(base.Owner.Player);
        IEnumerable<CardModel> source = pile.Cards.Where((CardModel c) => c.Type == CardType.Attack);
        IEnumerable<CardModel> enumerable = source.ToList().UnstableShuffle(base.Owner.Player.RunState.Rng.CombatCardSelection).Take(base.Amount);
        foreach (CardModel card in enumerable)
            await CardPileCmd.Add(card, PileType.Hand);
    }
}
