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
    private const int BaseTurns = 3;
    private const int ExtendTurns = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
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
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomFormchange();
            sora.PlayAnimation(ownerCreature, "cast");
            await Task.Delay((int)(0.2f * 1000f));
        }

        await CommonActions.CardBlock(this, play);

        var creature = base.Owner.Creature;
        int turns = creature.HasPower<UltimateFormPower>() ? ExtendTurns : BaseTurns;

        await PowerCmd.Apply<UltimateFormPower>(
            choiceContext,
            creature,
            turns,
            creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
