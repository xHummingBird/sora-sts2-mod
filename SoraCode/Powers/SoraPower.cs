using BaseLib.Abstracts;
using BaseLib.Extensions;
using Sora.SoraCode.Extensions;
using Godot;

namespace Sora.SoraCode.Powers;

public abstract class SoraPower : CustomPowerModel
{
    protected virtual string IconSuffix => "";

    protected string IconFileName =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}{IconSuffix}.png";

    public override string CustomPackedIconPath =>
        IconFileName.BigPowerImagePath();

    public override string CustomBigIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}