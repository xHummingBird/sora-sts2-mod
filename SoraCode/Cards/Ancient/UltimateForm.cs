using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Ancient;

public class UltimateForm() : SoraCard(0, CardType.Skill,
    CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Turns", 4),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<UltimateFormPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomFormchange();
            sora.PlayAnimation(ownerCreature, "cast");
            await Task.Delay((int)(0.2f * 1000f));
        }

        await PowerCmd.Apply<UltimateFormPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["Turns"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Turns"].UpgradeValueBy(1m);
    }
}
