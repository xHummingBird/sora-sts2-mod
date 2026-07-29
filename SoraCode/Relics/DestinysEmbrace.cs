using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Relics;

// At the start of your turn, if you have Kairi's Link, heal 2 HP.
public class DestinysEmbrace : SoraRelic
{
    private const int HealAmount = 2;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != base.Owner)
            return;

        if (!base.Owner.Creature.HasPower<KairiPower>())
            return;

        await CreatureCmd.Heal(base.Owner.Creature, HealAmount, true);
    }
}
