using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Common;

public class AerialSweep() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();
            sora.PlayAnimation(ownerCreature, "cast");
            await Task.Delay((int)(0.2f * 1000f));
        }

        int enemiesHit = base.CombatState.HittableEnemies.Count(e => e.IsAlive);

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_up.wav")
            .Execute(choiceContext);

        if (enemiesHit > 0)
        {
            SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            relic?.GainSituationPoints(enemiesHit);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
