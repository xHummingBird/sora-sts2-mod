using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Uncommon;

public class AerialRecovery() : SoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(4, ValueProp.Move),
        new CardsVar(1),
        new PowerVar<SituationReadyPower>(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
        CardModel cardModel = (await CardPileCmd.Draw(choiceContext, 1m, base.Owner)).FirstOrDefault();
        if (cardModel != null && cardModel.Type == CardType.Skill && relic != null) 
        {
            relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
        }
        
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["SituationReadyPower"].UpgradeValueBy(2m);
    }
}