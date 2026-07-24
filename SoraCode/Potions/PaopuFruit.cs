using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Potions;

// Gain 30 SP.
public class PaopuFruit : SoraPotion
{
    private const int SpGain = 30;

    public override PotionRarity Rarity => PotionRarity.Rare;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target)
    {
        SituationRelicBase? relic = base.Owner.GetRelic<SituationRelicBase>();
        relic?.GainSituationPoints(SpGain);

        await Task.CompletedTask;
    }
}
