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

public class StrikeShift() : SoraCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy), ICompanionCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(9, ValueProp.Move),
        new CardsVar(1),
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
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_sora_call.wav");
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            await Task.Delay((int)(0.1f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, hitOverride: "res://Sora/sfx/kairi/kairi_keyblade_hit (1).wav");
            await Task.Delay((int)(0.2f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, hitOverride: "res://Sora/sfx/kairi/kairi_keyblade_hit (2).wav");
            await Task.Delay((int)(0.2f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, hitOverride: "res://Sora/sfx/kairi/kairi_keyblade_hit (3).wav");
            await Task.Delay((int)(0.5 * 1000f));
            SfxCmd.Play("res://Sora/sounds/kairi/kairi_finish.wav");
            await Task.Delay((int)(0.1f * 1000f));
            SfxCmd.Play("res://Sora/sfx/swing_up.wav");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Sora/sfx/kairi/kairi_keyblade_hit (7).wav")
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner);
        await SoraExtensions.CombatHelpers.RefreshLink<KairiPower>(choiceContext, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}