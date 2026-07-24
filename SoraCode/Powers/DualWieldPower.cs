using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Sora.SoraCode.Powers;

// Each turn, the first attack card you play is played an additional time.
public class DualWieldPower : SoraPower
{
    private int _attacksPlayedThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override int ModifyCardPlayCount(
        CardModel card,
        Creature target,
        int playCount)
    {
        if (card.Owner.Creature != base.Owner)
            return playCount;

        if (card.Type != CardType.Attack)
            return playCount;

        // No attack has fully resolved this turn yet, so this is the first one.
        if (_attacksPlayedThisTurn != 0)
            return playCount;

        return playCount + 1;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner &&
            cardPlay.Card.Type == CardType.Attack)
        {
            _attacksPlayedThisTurn++;
        }

        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == base.Owner.Side)
            _attacksPlayedThisTurn = 0;

        await Task.CompletedTask;
    }
}
