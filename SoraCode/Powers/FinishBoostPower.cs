using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Powers;

// The third attack you play each turn deals extra damage.
// Amount is stored in "stacks" where each stack grants 1% more damage
// (50 stacks = +50%).
public class FinishBoostPower : SoraPower
{
    private int _attacksPlayedThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != base.Owner)
            return 1m;

        if (!props.IsPoweredAttack())
            return 1m;

        if (cardSource == null || cardSource.Type != CardType.Attack)
            return 1m;

        // At damage time two attacks have already fully resolved this turn,
        // so this is the third attack.
        if (_attacksPlayedThisTurn != 2)
            return 1m;

        return 1m + (decimal)base.Amount / 100m;
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
