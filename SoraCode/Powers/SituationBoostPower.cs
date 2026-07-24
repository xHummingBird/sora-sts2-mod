using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Powers;

// Whenever you play a card that costs 2 or more energy, gain 5 SP.
public class SituationBoostPower : SoraPower
{
    private const int SpGain = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != base.Owner)
            return;

        if (amount < 2)
            return;

        SituationRelicBase? relic = base.Owner.Player?.GetRelic<SituationRelicBase>();

        if (relic != null)
        {
            relic.GainSituationPoints(SpGain);
            Flash();
        }

        await Task.CompletedTask;
    }
}
