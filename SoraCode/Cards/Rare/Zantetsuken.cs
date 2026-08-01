using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class Zantetsuken() : SoraCard(2, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(18, ValueProp.Move),
        new DynamicVar("hpPercent", 10)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        var ownerCreature = Owner?.Creature;
        string attackVfx = base.Owner.Creature.HasPower<UltimateFormPower>()
            ? "hit_ultimate"
            : "atk_vfx";
        decimal threshold = DynamicVars["hpPercent"].BaseValue;

        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            
            await sora.DashTo(ownerCreature, play.Target, distance: 300f);
            AudioHelper.PlayRandomFinalAttack2();
            if (!ownerCreature.HasPower<UltimateFormPower>())
            {
                sora.PlayAnimation(ownerCreature, "attack");

                await Task.Delay((int)(0.2f * 1000f));
                sora.DashPast(base.Owner.Creature, play.Target, null, 0.19f);
                SfxCmd.Play("res://Sora/sfx/swing_down.wav");

                sora.PlayVfxOnTarget(
                    play.Target,
                    "res://Sora/scenes/vfx.tscn",
                    attackVfx
                );
                await CommonActions.CardAttack(this, play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_medium.wav")
                    .Execute(choiceContext);
                await Task.Delay((int)(0.2f * 1000f));
                sora.Retreat(ownerCreature);
            }

            else
            {
                sora.PlayAnimation(ownerCreature, "attack_ultimate_3");
                await Task.Delay((int)(0.2f * 1000f));
                SfxCmd.Play("res://Sora/sfx/ultimate_thrust.wav");
                sora.PlayVfxOnTarget(
                    play.Target,
                    "res://Sora/scenes/vfx.tscn",
                    attackVfx
                );
                CommonActions.CardAttack(this, play.Target)
                    .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/ultimate_hit_3.wav")
                    .Execute(choiceContext);
                await Task.Delay((int)(0.81f * 1000f));
                sora.Retreat(ownerCreature, null, true,0.01f);
                await Task.Delay((int)(0.18f * 1000f));
            }
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_slash", "res://Sora/sfx/hit_medium.wav")
                .Execute(choiceContext);
        if (play.Target.CurrentHp * 100 <= play.Target.MaxHp * threshold && play.Target.CurrentHp > 0)
        {
            await DoomPower.DoomKill(new List<Creature> { play.Target });
            return;
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars["hpPercent"].UpgradeValueBy(3m);
    }
}