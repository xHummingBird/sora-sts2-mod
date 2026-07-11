using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sora.SoraCode.Cards.Uncommon;

public class Brandish() : SoraCard(0, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<VigorPower>(3m),
    ];
    
    private bool CanDrawCard
    {
        get
        {
            int num = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card.Owner == base.Owner);
            return num < 3;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner.Creature, base.DynamicVars["VigorPower"].BaseValue,
            base.Owner.Creature, this);
        if (CanDrawCard)
        {
            await CardPileCmd.Draw(choiceContext, 1, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPower"].UpgradeValueBy(1m);
    }
}