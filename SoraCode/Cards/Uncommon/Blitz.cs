using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Uncommon;

public class Blitz() : SoraCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(9, ValueProp.Move),
        new PowerVar<SituationReadyPower>(3)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            AudioHelper.PlayRandomAttack();
            
            sora.PlayAnimation(ownerCreature, "blitz");
            
            await Task.Delay((int)(0.2f * 1000f));
            
            SfxCmd.Play("res://Sora/sfx/swing_down.wav");
            
            sora.PlayVfxOnTarget(
                play.Target,
                "res://Sora/scenes/vfx.tscn",
                "atk_vfx"
            );
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_medium.wav")
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