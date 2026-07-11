using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Powers;

public class DamageSyphonPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
            return;

        if (dealer == base.Owner)
            return;
    
        if (!props.IsPoweredAttack())
            return;
    
        int gain = 0;
    
        if (result.UnblockedDamage == 1)
        {
            gain += 1;
        }
        if (result.UnblockedDamage > 1)
        {
            gain += (int)(result.UnblockedDamage * 0.5);
        }
        SituationRelicBase? relic = Owner.Player?.GetRelic<SituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints(gain);
        }
    }
}