using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Rare;

public class SealKeyhole() : SoraCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<SituationReadyPower>(15)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SfxCmd.Play("res://Sora/sounds/hikari.wav");
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["SituationReadyPower"].UpgradeValueBy(5);
    }
}