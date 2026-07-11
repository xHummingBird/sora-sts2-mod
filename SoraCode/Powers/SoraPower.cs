using BaseLib.Abstracts;
using BaseLib.Extensions;
using Sora.SoraCode.Extensions;
using Godot;

namespace Sora.SoraCode.Powers;

public abstract class SoraPower : CustomPowerModel
{
    //Loads from Sora/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}