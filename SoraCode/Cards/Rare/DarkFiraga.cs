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
using Sora.SoraCode.Mechanics.Companion;

namespace Sora.SoraCode.Cards.Rare;

public class DarkFiraga() : SoraCard(2, CardType.Attack,
    CardRarity.Rare, TargetType.AllEnemies), ICompanionCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(14, ValueProp.Move),
        new PowerVar<VulnerablePower>(1),
        new PowerVar<WeakPower>(1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            AudioHelper.PlayRandomRiku();
            await Task.Delay((int)(0.3f * 1000f));
            var enemies = base.CombatState.HittableEnemies.ToList();

            if (enemies.Count == 0)
                return;

            var targetEnemy =
                enemies[(enemies.Count - 1) / 2];

            sora.PlayVfxOnTarget(
                targetEnemy,
                "res://Sora/scenes/riku.tscn",
                "dark_firaga");
            SfxCmd.Play("res://Sora/sounds/riku/dark_firaga.wav");
            await Task.Delay((int)(0.4f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/dark_firaga.wav");
            await Task.Delay((int)(0.2f * 1000f));
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitVfxSpawnedAtBase()
            .BeforeDamage(async delegate
            {
                var targets = base.CombatState.HittableEnemies;

                foreach (var target in targets)
                {
                    var vfx = NGroundFireVfx.Create(target, VfxColor.Purple);
                    if (vfx != null)
                    {
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                        SfxCmd.Play("event:/sfx/characters/attack_fire");
                    }
                }
            })
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Vulnerable.BaseValue,
            base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature, this);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars.Weak.UpgradeValueBy(1m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}