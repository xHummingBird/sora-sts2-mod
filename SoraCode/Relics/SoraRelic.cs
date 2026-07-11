using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Sora.SoraCode.Character;
using Sora.SoraCode.Extensions;
using Godot;

namespace Sora.SoraCode.Relics;

[Pool(typeof(SoraRelicPool))]
public abstract class SoraRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".BigRelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}