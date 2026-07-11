using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Sora.SoraCode.Cards.Rare;

public class BerserkCharge() : SoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
        {
            if (!card.EnergyCost.CostsX && card.Type == CardType.Attack)
            {
                card.SetToFreeThisTurn();
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}