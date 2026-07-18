using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Mechanics.Companion;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Common;

public class ShadowStrike() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy), ICompanionCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(5, ValueProp.Move),
        new PowerVar<VulnerablePower>(1),
        new PowerVar<WeakPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RikuPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
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
            AudioHelper.PlayRandomRiku();
            await Task.Delay((int)(0.3f * 1000f));
            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/riku.tscn",
                "shadow_strike"
            );
            await Task.Delay((int)(0.1f * 1000f));
            SfxCmd.Play("res://Sora/sounds/riku/riku_coop_2.wav");
            await Task.Delay((int)(0.365f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/riku_swing_down.wav");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Sora/sfx/riku/riku_hit_hard (2).wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, base.DynamicVars.Vulnerable.BaseValue,
            base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature, this);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        
        await SoraExtensions.CombatHelpers.RefreshLink<RikuPower>(choiceContext, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}