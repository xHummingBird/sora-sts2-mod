using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Rare;

public class SecondChance() : SoraCard(2, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(25m, ValueProp.Move),
        new PowerVar<SituationReadyPower>(12m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        SfxCmd.Play("res://Sora/sounds/mada.wav");
        if (base.Owner.Creature.CurrentHp * 2 < base.Owner.Creature.MaxHp)
        {
            SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            if (relic != null)
            {
                relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
        DynamicVars["SituationReadyPower"].UpgradeValueBy(3m);
    }
}