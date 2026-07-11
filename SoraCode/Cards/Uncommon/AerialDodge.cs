using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Uncommon;

public class AerialDodge() : SoraCard(2, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(12, ValueProp.Move),
        new PowerVar<SituationReadyPower>(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(4m);
    }
}