using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Powers;

// Duration buff. Amount stores the number of turns remaining. Decrements at the
// start of each of your turns and is removed when it reaches 0.
public class UltimateFormPower : SoraPower
{
    private const string _damageIncrease = "DamageIncrease";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageIncrease", 1.75m),
    ];
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool AllowNegative => false;
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack())
            return 1m;
        
        decimal num = base.DynamicVars["DamageIncrease"].BaseValue;
        
        if (dealer == base.Owner)
            return num;
        
        return 1m;
    }

    public override async Task AfterSideTurnStartLate(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Side)
            return;

        if (Amount <= 1)
        {
            await PowerCmd.Decrement(this);
            var ownerCreature = Owner;
            if (ownerCreature != null && Owner.Player.Character is Character.Sora sora)
                sora.PlayAnimation(ownerCreature, "idle_normal");
        }
        else
            await PowerCmd.Decrement(this);
    }
}
