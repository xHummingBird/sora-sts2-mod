using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Powers;

// Whenever you play an attack card, deal damage to a random enemy.
// Amount stores the damage dealt.
public class SynchBladePower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        var target =
            base.CombatState.HittableEnemies
                .Where(e => e.IsAlive)
                .ToList()
                .StableShuffle(base.Owner.Player.RunState.Rng.Shuffle)
                .FirstOrDefault();

        if (target == null)
            return;

        Flash();

        await CreatureCmd.Damage(
            choiceContext,
            target,
            (decimal)base.Amount,
            ValueProp.Unpowered,
            base.Owner);
    }
}
