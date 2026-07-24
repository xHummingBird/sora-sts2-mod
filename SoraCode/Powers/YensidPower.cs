using MegaCrit.Sts2.Core.Entities.Powers;

namespace Sora.SoraCode.Powers;

public class YensidPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}