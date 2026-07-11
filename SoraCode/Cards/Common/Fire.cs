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

public class Fire() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
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
            AudioHelper.PlayRandomFire();
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.2f * 1000f));
        }
        await CommonActions.CardAttack(this, play.Target)
            .BeforeDamage(async delegate
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(play.Target));
                SfxCmd.Play("event:/sfx/characters/attack_fire");
            })
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, base.DynamicVars.Vulnerable.BaseValue,
            base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}