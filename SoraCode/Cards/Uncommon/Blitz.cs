using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Uncommon;

public class Blitz() : SoraCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => PlayCountThisTurn == 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(8, ValueProp.Move),
        new PowerVar<SituationReadyPower>(2),
        new RepeatVar(2)
    ];
    
    private int PlayCountThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Count(e =>
                e.HappenedThisTurn(base.CombatState) &&
                e.CardPlay.Player == base.Owner);
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        int hitCount = 1;
        if (PlayCountThisTurn == 0)
        {
            hitCount = 2;
        }
        
        string hitSfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "res://Sora/sfx/ultimate_hit_1.wav"
            : "res://Sora/sfx/hit_medium.wav";
        
        string attackAnim = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "attack_ultimate"
            : "blitz";
        
        string attackVfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "hit_ultimate"
            : "atk_vfx";

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();
            
            sora.PlayAnimation(ownerCreature, attackAnim);
            
            await Task.Delay((int)(0.2f * 1000f));
            
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            
            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/vfx.tscn",
                attackVfx
            );
        }
        await CommonActions.CardAttack(this, play.Target, hitCount: hitCount)
            .WithHitFx("vfx/vfx_attack_slash", hitSfx)
            .Execute(choiceContext);
        SituationRelicBase? relic = Owner.GetRelic<SituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints((int)DynamicVars["SituationReadyPower"].BaseValue);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["SituationReadyPower"].UpgradeValueBy(1);
    }
}