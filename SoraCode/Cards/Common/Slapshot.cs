using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Common;

public class Slapshot() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("BlockPerEnemy", 2),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        int enemiesHit = base.CombatState.HittableEnemies.Count(e => e.IsAlive);
        var targets = base.CombatState.HittableEnemies;
        
        string hitSfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "res://Sora/sfx/ultimate_hit_1.wav"
            : "res://Sora/sfx/hit_medium.wav";

        string attackAnim = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "attack_ultimate_2"
            : "sonic_strike";

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();
            
            sora.PlayAnimation(ownerCreature, attackAnim);
            
            await Task.Delay((int)(0.133f * 1000f));
            
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            foreach (var target in targets)
            {
                sora.PlayVfxOnTarget(
                    target,
                    "res://Sora/scenes/vfx.tscn",
                    "atk_vfx"
                );
            }
        }

        

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", hitSfx)
            .Execute(choiceContext);

        int block = enemiesHit * (int)DynamicVars["BlockPerEnemy"].BaseValue;

        if (block > 0)
        {
            await CreatureCmd.GainBlock(
                base.Owner.Creature,
                block,
                ValueProp.Move,
                play,
                false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["BlockPerEnemy"].UpgradeValueBy(1m);
    }
}
