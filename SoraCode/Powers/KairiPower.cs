using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Powers;

public class KairiPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;
    
    protected override string IconSuffix => "_small";

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override bool AllowNegative => false;
    
    public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Side)
            return;
        
        await PowerCmd.Decrement(this);
    }
    
    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != null)
        {
            if (cardSource.Owner.Creature != base.Owner)
            {
                return 0m;
            }
        }
        else if (base.Owner != target)
        {
            return 0m;
        }
        if (!props.IsPoweredCardOrMonsterMoveBlock())
        {
            return 0m;
        }
        return 2;
    }
}