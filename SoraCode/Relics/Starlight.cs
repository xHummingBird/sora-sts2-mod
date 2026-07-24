using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Sora.SoraCode.Relics;

// Start combat with 5 SP.
public class Starlight : SoraRelic
{
    private const int StartingSp = 5;

    private bool _appliedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Common;

    public override Task BeforeCombatStart()
    {
        _appliedThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (_appliedThisCombat || player != base.Owner.Player)
            return Task.CompletedTask;

        _appliedThisCombat = true;

        SituationRelicBase? relic = base.Owner.GetRelic<SituationRelicBase>();
        relic?.GainSituationPoints(StartingSp);

        return Task.CompletedTask;
    }
}
