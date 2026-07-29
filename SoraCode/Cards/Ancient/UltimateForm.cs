using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.SituationCommand;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Ancient;

public class UltimateForm() : SoraCard(0, CardType.Skill,
    CardRarity.Ancient, TargetType.Self), ISituationCard
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

        if (ownerCreature == null)
            return;

        bool hasUltimate = ownerCreature.HasPower<UltimateFormPower>();

        if (Owner?.Character is Character.Sora sora && !hasUltimate)
        {
            AudioHelper.PlayRandomFormchange();
            sora.PlayAnimation(ownerCreature, "ultimate_form");
            SfxCmd.Play("res://Sora/sfx/ultimate_form.mp3");

            PowerCmd.Apply<UltimateFormPower>(
                choiceContext,
                ownerCreature,
                DynamicVars["Turns"].BaseValue,
                ownerCreature,
                this);
            await Task.Delay(2265);
            sora.PlayAnimation(ownerCreature, "idle_ultimate");
        }
        else
        {
            await PowerCmd.Apply<UltimateFormPower>(
                choiceContext,
                ownerCreature,
                DynamicVars["Turns"].BaseValue,
                ownerCreature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Turns"].UpgradeValueBy(1m);
    }
}
