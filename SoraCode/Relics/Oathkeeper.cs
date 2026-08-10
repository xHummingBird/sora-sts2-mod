using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Relics;

// At the start of combat, gain Kairi's Link (3 turns).
public class Oathkeeper : SoraRelic
{
    private const int LinkTurns = 3;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
        {
            await PowerCmd.Apply<KairiPower>(
                new ThrowingPlayerChoiceContext(),
                base.Owner.Creature,
                LinkTurns,
                base.Owner.Creature,
                null);
        }
    }
}
