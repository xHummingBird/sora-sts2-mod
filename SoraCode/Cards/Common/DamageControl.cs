using System;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Cards.Common;

public class DamageControl() : SoraCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("HealPercent", 20),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var creature = base.Owner.Creature;
        decimal missingHp = creature.MaxHp - creature.CurrentHp;

        if (missingHp > 0)
        {
            decimal percent = DynamicVars["HealPercent"].BaseValue;
            int heal = (int)Math.Floor(missingHp * percent / 100m);

            if (heal > 0)
            {
                await CreatureCmd.Heal(creature, heal, true);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HealPercent"].UpgradeValueBy(10m);
    }
}
