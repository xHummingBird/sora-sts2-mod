using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Powers;

// Whenever you play an attack card, deal damage to a random enemy.
// Amount stores the damage dealt.
public class SynchBladePower : SoraPower
{
    private class Data
    {
        public readonly Dictionary<CardModel, int>
            AmountsForPlayedCards = new();
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeCardPlayed(
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        GetInternalData<Data>()
            .AmountsForPlayedCards
            .Add(cardPlay.Card, Amount);

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
            return;

        if (!GetInternalData<Data>()
                .AmountsForPlayedCards
                .Remove(
                    cardPlay.Card,
                    out var damage))
        {
            return;
        }

        if (damage <= 0)
            return;

        Creature creature =
            base.Owner.Player.RunState.Rng
                .CombatTargets.NextItem(
                    base.Owner.CombatState
                        ?.HittableEnemies ??
                    Array.Empty<Creature>());

        if (creature == null)
            return;

        Flash();

        await CreatureCmd.Damage(
            choiceContext,
            creature,
            damage,
            ValueProp.Unpowered,
            base.Owner);
    }
}

