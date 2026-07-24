using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Relics;

// At the start of combat, gain Cloud's Link (3 turns).
public class Fenrir : SoraRelic
{
    private const int LinkTurns = 3;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<CloudPower>(
            null,
            base.Owner.Creature,
            LinkTurns,
            base.Owner.Creature,
            null);
    }
}
