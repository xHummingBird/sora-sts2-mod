using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Cards.Uncommon;

public class Counterguard() : SoraCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(5, ValueProp.Move),
        new PowerVar<VigorPower>(5m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner.Creature, base.DynamicVars["VigorPower"].BaseValue,
            base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(2m);
        DynamicVars["VigorPower"].UpgradeValueBy(2m);
    }
}