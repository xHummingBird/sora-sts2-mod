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

namespace Sora.SoraCode.Cards.Uncommon;

public class SpiralBloom() : SoraCard(2, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy), ICompanionCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(15, ValueProp.Move),
        new PowerVar<WeakPower>(1),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<KairiPower>(),
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
            AudioHelper.PlayRandomKairi();
            await Task.Delay((int)(0.3f * 1000f));
            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/kairi.tscn",
                "strike_shift"
            );
            await Task.Delay((int)(0.1f * 1000f));
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_finish_3.wav");
            await Task.Delay((int)(0.1f * 1000f));
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, hitOverride: "res://Sora/sfx/kairi/kairi_keyblade_hit (1).wav");
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            SoraExtensions.CombatHelpers.FakeHit(play.Target, hitOverride: "res://Sora/sfx/kairi/kairi_keyblade_hit (2).wav");
            await Task.Delay((int)(0.4f * 1000f));
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_finish_2.wav");
            await Task.Delay((int)(0.067f * 1000f));
            SfxCmd.Play("res://Sora/sfx/swing_up.wav");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Sora/sfx/kairi/kairi_keyblade_hit (3).wav")
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature, this);
        await SoraExtensions.CombatHelpers.RefreshLink<KairiPower>(choiceContext, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}