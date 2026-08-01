using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.SituationCommand;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Ancient;

public class UltimateFinisher() : SoraCard(3, CardType.Attack,
    CardRarity.Ancient, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(45, ValueProp.Move),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    private IEnumerable<CardModel> GetSituationCommandCard()
    {
        var pile = PileType.Hand.GetPile(base.Owner);
        return pile.Cards.OfType<SituationCommand>();
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var relic = Owner.Relics
            .OfType<SituationRelicBase>()
            .FirstOrDefault();

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            
            void PlayFakeHitAll(
                IReadOnlyList<Creature> targets)
            {
                foreach (var target in targets)
                {
                    if (!target.IsAlive)
                        continue;

                    SoraExtensions.CombatHelpers.FakeHit(target);
                    
                }
            }
            
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            var enemies = base.CombatState.HittableEnemies.ToList();

            var targetEnemy =
                enemies[(enemies.Count - 1) / 2];
            
            await sora.DashTo(ownerCreature, targetEnemy, distance: 100f);
            if (ownerCreature.HasPower<SituationReadyPower>())
            {
                foreach (var card in GetSituationCommandCard().ToList())
                {
                    await CardCmd.Exhaust(choiceContext, card);
                }
                await PowerCmd.Remove<SituationReadyPower>(Owner.Creature);
                SfxCmd.Play("res://Sora/sfx/formchange.wav");
            }
            relic?.SetSituationPoints(0);
            
            sora.PlayAnimation(ownerCreature, "ultimate_finish");
            SfxCmd.Play("res://Sora/sounds/hikari.wav");
            SfxCmd.Play("res://Sora/sfx/ultimate_finish_before.wav");
            
            await Task.Delay((int)(0.9667f * 1000f));
            
            SfxCmd.Play("res://Sora/sfx/ultimate_finish_atk.wav");
            SfxCmd.Play("res://Sora/sfx/ultimate_finish_swing.wav");
            PlayFakeHitAll(enemies);

            for (int i = 0; i < 8; i++)
            {
                await Task.Delay((int)(0.2f * 1000f));
                PlayFakeHitAll(enemies);
            }

            
            await Task.Delay((int)(0.7f * 1000f));
            SfxCmd.Play("res://Sora/sfx/ultimate_finish_finish.wav");
            SfxCmd.Play("res://Sora/sounds/finalhit2_6.wav");
            
            await Task.Delay((int)(0.7f * 1000f));
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
            if (ownerCreature.HasPower<UltimateFormPower>())
                await PowerCmd.Remove<UltimateFormPower>(ownerCreature);
            await Task.Delay((int)(2f * 1000f));
            await sora.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
        {
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
            if (ownerCreature.HasPower<UltimateFormPower>())
                await PowerCmd.Remove<UltimateFormPower>(ownerCreature);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}