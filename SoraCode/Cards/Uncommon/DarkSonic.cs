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
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Uncommon;

public class DarkSonic() : SoraCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy), ICompanionCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<SituationReadyPower>(3),
        new DamageVar(11, ValueProp.Move),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RikuPower>(),
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
                "dark_sonic"
            );
            await Task.Delay((int)(0.1f * 1000f));
            SfxCmd.Play("res://Sora/sounds/riku/riku_coop_2.wav");
            await Task.Delay((int)(0.1f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, null, null, "res://Sora/sfx/riku/riku_swing_down.wav", "res://Sora/sfx/riku/riku_hit_1.wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "riku_atk_vfx");
            await Task.Delay((int)(0.467f * 1000f));
            SfxCmd.Play("res://Sora/sfx/riku/riku_thrust.wav");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Sora/sfx/riku/riku_hit_hard (2).wav")
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
        if (relic != null)
        {
            relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
        }
        await SoraExtensions.CombatHelpers.RefreshLink<RikuPower>(choiceContext, base.Owner.Creature, this);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}