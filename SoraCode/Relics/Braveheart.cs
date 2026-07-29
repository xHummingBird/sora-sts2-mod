using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Relics;

// At the start of your turn, if you have Riku's Link, gain 4 Vigor.
public class Braveheart : SoraRelic
{
    private const int VigorGain = 4;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != base.Owner)
            return;

        if (!base.Owner.Creature.HasPower<RikuPower>())
            return;

        await PowerCmd.Apply<VigorPower>(
            choiceContext,
            base.Owner.Creature,
            VigorGain,
            base.Owner.Creature,
            null);
    }
}
