using MegaCrit.Sts2.Core.Entities.Powers;

namespace Sora.SoraCode.Powers;

public class KairiPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}