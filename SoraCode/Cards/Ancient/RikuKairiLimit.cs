using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.SituationCommand;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Ancient;

public class RikuKairiLimit() : SoraCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AllEnemies), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(40, ValueProp.Move),
        new HealVar(7m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            AudioHelper.PlayRandomTrueEnd();
            sora.PlayAnimation(ownerCreature, "true_end_cast");
            
            await Task.Delay((int)(1.2f * 1000f));
            
            SfxCmd.Play("res://Sora/sounds/hikari.wav");
            SfxCmd.Play("res://Sora/sounds/riku/riku_hikari.wav");
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_hikari.wav");
            
            await Task.Delay((int)(0.19f * 1000f));
            
            sora.PlayAnimation(ownerCreature, "true_end_attack");
            var enemies = base.CombatState.HittableEnemies.ToList();

            if (enemies.Count == 0)
                return;

            var targetEnemy =
                enemies[(enemies.Count - 1) / 2];

            sora.PlayVfxOnTarget(
                targetEnemy,
                "res://Sora/scenes/vfx.tscn",
                "true_end");
            SfxCmd.Play("res://Sora/sfx/ragnarok.wav");
            await Task.Delay((int)(1.533f * 1000f));
            SfxCmd.Play("res://Sora/sfx/ragnarok_shoot.wav");
            await Task.Delay((int)(0.2f * 1000f));
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitVfxSpawnedAtBase()
            .BeforeDamage(async delegate
            {
                var targets = base.CombatState.HittableEnemies;

                foreach (var target in targets)
                {
                    var vfx = NGroundFireVfx.Create(target, VfxColor.White);
                    if (vfx != null)
                    {
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                        SfxCmd.Play("event:/sfx/characters/attack_fire");
                    }
                }
            })
            .Execute(choiceContext);
        await Task.Delay((int)(0.9f * 1000f));
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await CreatureCmd.Heal(base.Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
        DynamicVars.Heal.UpgradeValueBy(3m);
    }
}