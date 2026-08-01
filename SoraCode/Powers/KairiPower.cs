using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

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
}