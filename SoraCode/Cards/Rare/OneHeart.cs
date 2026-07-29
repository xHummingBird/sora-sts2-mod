using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;

namespace Sora.SoraCode.Cards.Rare;

public class OneHeart() : SoraCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("BlockPerLink", 5),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var creature = base.Owner.Creature;

        int activeLinks = SoraExtensions.CombatHelpers.CountActiveLinks(creature);

        decimal block =
            DynamicVars.Block.BaseValue +
            activeLinks * DynamicVars["BlockPerLink"].BaseValue;

        await CreatureCmd.GainBlock(
            creature,
            block,
            ValueProp.Move,
            play,
            false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars["BlockPerLink"].UpgradeValueBy(1m);
    }
}
