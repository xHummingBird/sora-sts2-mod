using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Cards.Basic;
using Sora.SoraCode.Mechanics.SituationCommand;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Cards.Ancient;

public class SituationCommand() : SoraCard(1, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override bool IsPlayable => base.Owner.HasPower<SituationReadyPower>();
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var relic = Owner.Relics
            .OfType<SituationRelicBase>()
            .FirstOrDefault();

        await PowerCmd.Remove<SituationReadyPower>(Owner.Creature);

        relic?.MarkSituationReadyConsumedThisTurn();

        if (relic == null)
            return;

        if (relic.SituationPoints >= 60)
        {
            CardModel finisher =
                base.CombatState.CreateCard<SonicBlade>(Owner);

            // Companion replacements
            if (Owner.Creature.HasPower<RikuPower>())
            {
                finisher =
                    base.CombatState.CreateCard<RikuLimit>(Owner);
            }

            // Future companions
            // else if (Owner.Creature.HasPower<KairiPower>())
            // {
            //     finisher =
            //         base.CombatState.CreateCard<KairiLimit>(Owner);
            // }

            var arsArcanum =
                base.CombatState.CreateCard<ArsArcanum>(Owner);

            var cards = new List<CardModel>
            {
                finisher,
                arsArcanum
            };

            var selectedCard =
                await CardSelectCmd.FromChooseACardScreen(
                    choiceContext,
                    cards,
                    Owner,
                    canSkip: false);

            if (selectedCard is SonicBlade)
            {
                relic.SpendSituationPoints(30);

                await CardCmd.AutoPlay(
                    choiceContext,
                    selectedCard,
                    null);
            }
            else
            {
                relic.SpendSituationPoints(60);

                await CardCmd.AutoPlay(
                    choiceContext,
                    selectedCard,
                    play.Target);
            }
        }
        else if (relic.SituationPoints >= 30)
        {
            relic.SpendSituationPoints(30);

            var sonicBlade =
                base.CombatState.CreateCard<SonicBlade>(Owner);

            await CardCmd.AutoPlay(
                choiceContext,
                sonicBlade,
                null);
        }
    }
    
}