using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Ancient;

public class Formchange() : SoraCard(2, CardType.Skill,
    CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new DynamicVar("Turns", 3),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<UltimateFormPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await Task.Delay((int)(0.1f * 1000f));
        var ownerCreature = Owner?.Creature;
        bool hasUltimate = ownerCreature.HasPower<UltimateFormPower>();
        if (ownerCreature != null && Owner?.Character is Character.Sora sora && !hasUltimate)
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
                DynamicVars["Turns"].BaseValue - 1,
                ownerCreature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
