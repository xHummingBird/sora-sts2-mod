using MegaCrit.Sts2.Core.Entities.Powers;

namespace Sora.SoraCode.Powers;

public class CloudPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;
    
    protected override string IconSuffix => "_small";

    public override PowerStackType StackType => PowerStackType.Counter;
}