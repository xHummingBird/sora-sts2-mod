using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sora.SoraCode.Mechanics.Companion;

namespace Sora.SoraCode.Powers;

// The first time you play a Companion card each turn, gain Energy.
// Amount stores the Energy gained (stacks).
public class SevenLightsPower : SoraPower
{
    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (_triggeredThisTurn)
            return;

        if (cardPlay.Card.Owner.Creature != base.Owner)
            return;

        if (cardPlay.Card is not ICompanionCard)
            return;

        _triggeredThisTurn = true;

        Flash();

        await PlayerCmd.GainEnergy(
            (decimal)base.Amount,
            base.Owner.Player);
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player == base.Owner.Player)
            _triggeredThisTurn = false;

        await Task.CompletedTask;
    }
}
