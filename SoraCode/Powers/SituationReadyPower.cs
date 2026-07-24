using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Cards.Ancient;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Powers;

public class SituationReadyPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        CardModel card = CombatState.CreateCard<SituationCommand>(base.Owner.Player);

        // Wayfinder makes the generated Situation Command free and upgraded.
        if (base.Owner.Player?.GetRelic<Wayfinder>() != null)
        {
            card.EnergyCost.SetCustomBaseCost(0);
            await CardCmd.Upgrade(card, default);
        }

        await Task.Delay((int)(0.50f * 1000f));
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner.Player);
        Flash();
    }
}
