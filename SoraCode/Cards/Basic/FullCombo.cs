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
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Basic;

public class FullCombo() : SoraCard(2, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DamageVar(13, ValueProp.Move)
        ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await sora.DashTo(ownerCreature, play.Target, distance: 350f);
            AudioHelper.PlayRandomAttack();
            
            float duration = sora.PlayAnimation(ownerCreature, "full_combo").total;
            if (duration > 0f)
                await Task.Delay((int)(0.13f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, SoraExtensions.SwingSfx.SwingDown, SoraExtensions.HitSfx.HitDown);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.266f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target);
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.3333f * 1000f));
            SoraExtensions.CombatHelpers.FakeHit(play.Target, SoraExtensions.SwingSfx.SwingDown, hitOverride:"res://Sora/sfx/hit_medium.wav");
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await Task.Delay((int)(0.4f * 1000f));
            AudioHelper.PlayRandomFinalAttack();
            await Task.Delay((int)(0.467f * 1000f));
            sora.PlayVfxOnTarget(play.Target, "res://Sora/scenes/vfx.tscn", "atk_vfx");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
                .Execute(choiceContext);
            await Task.Delay((int)(0.36f * 1000f));
            await sora.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
            SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            
            if (relic != null)
            {
                relic.GainSituationPoints(4);
            }
        }
        else  
            await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_hard.wav")
            .Execute(choiceContext);
        
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}