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

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner)
            return playCount;

        if (card.Type is not CardType.Attack)
            return playCount;

        int numAttackPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count(
            e =>
                e.Actor == base.Owner &&
                e.CardPlay.IsFirstInSeries &&
                e.HappenedThisTurn(base.CombatState) &&
                e.CardPlay.Card.Type is CardType.Attack
        );

        if (numAttackPlayedThisTurn >= base.Amount)
            return playCount;

        return playCount + 1;
    }


    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}
