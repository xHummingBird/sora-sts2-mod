using MegaCrit.Sts2.Core.Combat;
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
            CardCmd.Upgrade(card, default);
        }

        await Task.Delay((int)(0.50f * 1000f));
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner.Player);
        Flash();
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Side)
            return;

        var player = Owner.Player;
        var playerState = player.PlayerCombatState;

        if (playerState == null)
            return;
        
        if (playerState.AllCards
            .OfType<SituationCommand>()
            .Any(c => c.Pile?.Type == PileType.Hand))
        {
            return;
        }

        var cards = playerState.AllCards
            .OfType<SituationCommand>()
            .Where(c => c.Pile == null || c.Pile.Type != PileType.Hand);
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}
