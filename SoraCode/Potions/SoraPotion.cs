using BaseLib.Abstracts;
using BaseLib.Extensions;
using Sora.SoraCode.Extensions;

public abstract class SoraPotion : CustomPotionModel
{
    protected string PotionFileName =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    protected string PotionOutlineFileName =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";

    public override string? CustomPackedImagePath =>
        PotionFileName.PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        PotionOutlineFileName.PotionImagePath();
}