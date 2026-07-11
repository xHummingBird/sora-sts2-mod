using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class FlowMotion() : SoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(2),
        new EnergyVar(2),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards =
            await CardPileCmd.Draw(
                choiceContext,
                DynamicVars.Cards.IntValue,
                base.Owner);

        if (cards.Any(c => c.Type == CardType.Attack))
        {
            await PlayerCmd.GainEnergy(
                base.DynamicVars.Energy.IntValue,
                base.Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}