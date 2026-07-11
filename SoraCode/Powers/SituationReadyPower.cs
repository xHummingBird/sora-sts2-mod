using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Cards.Ancient;

namespace Sora.SoraCode.Powers;

public class SituationReadyPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        CardModel card = CombatState.CreateCard<SituationCommand>(base.Owner.Player);
        await Task.Delay((int)(0.50f * 1000f));
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner.Player);
        Flash();
    }
}