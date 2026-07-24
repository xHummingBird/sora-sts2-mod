using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.SituationCommand;

namespace Sora.SoraCode.Cards.Ancient;

public class SonicBlade() : SoraCard(
    0,
    CardType.Attack,
    CardRarity.Ancient,
    TargetType.AllEnemies),
    ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move)
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

        if (ownerCreature == null)
            return;

        var enemies = base.CombatState.HittableEnemies
            .Where(enemy => enemy.IsAlive)
            .ToList();

        if (enemies.Count == 0)
            return;

        if (Owner?.Character is Character.Sora sora)
        {
            var room = NCombatRoom.Instance;
            if (room == null)
                return;
            var soraNode = room.GetCreatureNode(ownerCreature);
            if (soraNode == null)
                return;

            Vector2 originalPosition = soraNode.GlobalPosition;

            Vector2? leftMostEnemyBottom = null;
            Vector2? rightMostEnemyBottom = null;

            foreach (var enemy in enemies)
            {
                var enemyNode = room.GetCreatureNode(enemy);

                if (enemyNode == null)
                    continue;

                Vector2 bottom = enemyNode.GetBottomOfHitbox();

                if (leftMostEnemyBottom == null ||
                    bottom.X < leftMostEnemyBottom.Value.X)
                {
                    leftMostEnemyBottom = bottom;
                }

                if (rightMostEnemyBottom == null ||
                    bottom.X > rightMostEnemyBottom.Value.X)
                {
                    rightMostEnemyBottom = bottom;
                }
            }

            if (leftMostEnemyBottom == null ||
                rightMostEnemyBottom == null)
            {
                return;
            }

            const float leftDistance = 400f;
            const float rightDistance = 350f;

            Vector2 leftSidePosition = new Vector2(
                leftMostEnemyBottom.Value.X - leftDistance,
                originalPosition.Y);

            Vector2 rightSidePosition = new Vector2(
                rightMostEnemyBottom.Value.X + rightDistance,
                originalPosition.Y);
            
            async Task TimedDashTo(
                Vector2 destination,
                float duration)
            {
                var tween = soraNode.CreateTween();

                tween.TweenProperty(
                        soraNode,
                        "global_position",
                        destination,
                        duration)
                    .SetTrans(Tween.TransitionType.Quad)
                    .SetEase(Tween.EaseType.Out);

                await soraNode.ToSignal(
                    tween,
                    Tween.SignalName.Finished);
            }

            void PlayFakeHitAll(
                IReadOnlyList<Creature> targets,
                Character.Sora soraCharacter)
            {
                foreach (var target in targets)
                {
                    if (!target.IsAlive)
                        continue;

                    SoraExtensions.CombatHelpers.FakeHit(target);

                    soraCharacter.PlayVfxOnTarget(
                        target,
                        "res://Sora/scenes/vfx.tscn",
                        "atk_vfx"
                    );
                }
            }

            async Task DashWithFakeHit(
                Vector2 destination)
            {
                TimedDashTo(
                    destination,
                    0.2f);

                await Task.Delay((int)(0.067f * 1000f));

                PlayFakeHitAll(
                    enemies,
                    sora);

                await Task.Delay((int)(0.133f * 1000f));
            }
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await sora.DashToPosition(
                ownerCreature,
                leftSidePosition,
                durationSeconds: 0.30f,
                overrideAnim: null);

            sora.PlayAnimation(ownerCreature, "sonic_blade");

            AudioHelper.PlayRandomFormchange();

            await Task.Delay((int)(0.2f * 1000f));

            SfxCmd.Play("res://Sora/sfx/formchange_3.wav");

            await Task.Delay((int)(0.833f * 1000f));

            AudioHelper.PlayRandomAttack();

            await Task.Delay((int)(0.267f * 1000f));

// Dash 1: left -> right
            await DashWithFakeHit(rightSidePosition);

// Dash 2: right -> left
            await Task.Delay((int)(0.067f * 1000f));
            await DashWithFakeHit(leftSidePosition);

// Dash 3: left -> right
            await Task.Delay((int)(0.067f * 1000f));
            AudioHelper.PlayRandomAttack();
            await DashWithFakeHit(rightSidePosition);

// Dash 4: right -> left
            await Task.Delay((int)(0.067f * 1000f));
            await DashWithFakeHit(leftSidePosition);

// Dash 5: left -> right
            await Task.Delay((int)(0.067f * 1000f));
            AudioHelper.PlayRandomAttack();
            await DashWithFakeHit(rightSidePosition);

// Dash 6: right -> left
            await Task.Delay((int)(0.067f * 1000f));
            await DashWithFakeHit(leftSidePosition);

// Special audio before final dash
            await Task.Delay((int)(0.067f * 1000f));

            SfxCmd.Play("res://Sora/sounds/finalhit_9.wav");
            SfxCmd.Play("res://Sora/sfx/sonic_blade.wav");

            await Task.Delay((int)(0.2f * 1000f));

// Final dash: left -> right
            var finalDashTask = TimedDashTo(
                rightSidePosition,
                0.2f);

            await Task.Delay((int)(0.067f * 1000f));
            
            foreach (var target in base.CombatState.HittableEnemies)
            {
                sora.PlayVfxOnTarget(
                    target,
                    "res://Sora/scenes/vfx.tscn",
                    "atk_vfx"
                );
            }
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx(
                    "vfx/vfx_attack_slash",
                    "res://Sora/sfx/hit_up.wav")
                .Execute(choiceContext);

            await finalDashTask;
            await sora.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
    }
    protected override void OnUpgrade()
    {
    }
}