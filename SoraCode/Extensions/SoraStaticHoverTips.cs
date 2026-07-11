using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Sora.SoraCode.Extensions;

public class SoraStaticHoverTips
{
    public static readonly IHoverTip SP = new HoverTip(
        new LocString("static_hover_tips", "SORA_SP.title"),
        new LocString("static_hover_tips", "SORA_SP.description")
    );
}