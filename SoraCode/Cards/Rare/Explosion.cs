using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;

namespace Sora.SoraCode.Cards.Rare;

public class Explosion() : SoraCard(2, CardType.Attack,
    CardRarity.Rare, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DamageVar(5, ValueProp.Move),
            new RepeatVar(5)
        ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            // attack animation
            float duration = sora.PlayAnimation(ownerCreature, "cast").total;
            AudioHelper.PlayRandomFinalAttack();
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.2f * 1000f));
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitVfxNode(target =>
            {
                SfxCmd.Play("event:/sfx/characters/attack_fire");
                return NGroundFireVfx.Create(target);
            })
            .Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}