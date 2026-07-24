using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sora.SoraCode.Powers;

// Reduces SP gained per turn by 1 (handled in SituationRelicBase) and grants
// Strength at the start of each of your turns. Amount stores the Strength
// gained per turn.
public class NegativeComboPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != base.Owner.Player)
            return;

        Flash();

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            base.Owner,
            (decimal)base.Amount,
            base.Owner,
            null);
    }
}
