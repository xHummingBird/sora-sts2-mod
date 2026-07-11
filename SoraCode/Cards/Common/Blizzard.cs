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
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;

namespace Sora.SoraCode.Cards.Common;

public class Blizzard() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<WeakPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
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
            AudioHelper.PlayRandomBlizzard();
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                sora.PlayVfxOnTarget(
                    play.Target,
                    "res://Sora/scenes/ice_vfx.tscn",
                    "ice_1"
                );
            await Task.Delay((int)(0.20f * 1000f));
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Sora/sfx/ice.wav")
            .BeforeDamage(async delegate
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(play.Target, VfxColor.Blue));
            })
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}