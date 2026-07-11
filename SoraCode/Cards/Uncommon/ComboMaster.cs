using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Uncommon;

public class ComboMaster() : SoraCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<SituationReadyPower>(3),
        new EnergyVar(1)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
        }
        await PowerCmd.Apply<FreeAttackPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["SituationReadyPower"].UpgradeValueBy(3m);
    }

}