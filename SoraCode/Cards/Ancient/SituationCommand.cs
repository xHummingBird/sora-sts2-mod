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

public class SituationCommand() : SoraCard(1, CardType.Skill,
    CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override bool IsPlayable => base.Owner.HasPower<SituationReadyPower>();
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    private IEnumerable<CardModel> GetUltimateFormCard()
    {
        var pile = PileType.Hand.GetPile(base.Owner);
        return pile.Cards.OfType<UltimateForm>();
    }
    
    private IEnumerable<CardModel> GetSituationCommandCard()
    {
        var pile = PileType.Hand.GetPile(base.Owner);
        return pile.Cards.OfType<SituationCommand>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var relic = Owner.Relics
            .OfType<SituationRelicBase>()
            .FirstOrDefault();

        Wayfinder? wayfinder = base.Owner.GetRelic<Wayfinder>();
        
        foreach (var card in GetUltimateFormCard().ToList())
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        await PowerCmd.Remove<SituationReadyPower>(Owner.Creature);
            
        foreach (var card in GetSituationCommandCard().ToList())
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        
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
            
            CardModel finisher2 =
                base.CombatState.CreateCard<ArsArcanum>(Owner);
            
            if (Owner.Creature.HasPower<RikuPower>() && Owner.Creature.HasPower<KairiPower>())
                finisher2 = base.CombatState.CreateCard<RikuKairiLimit>(Owner);
            
            else if (Owner.Creature.HasPower<KairiPower>())
            {
                finisher2 =
                    base.CombatState.CreateCard<KairiLimit>(Owner);
            }

            if (wayfinder != null)
            {
                CardCmd.Upgrade(finisher);
                CardCmd.Upgrade(finisher2);
            }
            
            var cards = new List<CardModel>
            {
                finisher,
                finisher2
            };

            var selectedCard =
                await CardSelectCmd.FromChooseACardScreen(
                    choiceContext,
                    cards,
                    Owner,
                    canSkip: false);

            if (selectedCard is SonicBlade)
            {
                SfxCmd.Play("res://Sora/sfx/formchange.wav");
                relic.SpendSituationPoints(30);

                await CardCmd.AutoPlay(
                    choiceContext,
                    selectedCard,
                    null);
            }
            else
            {
                SfxCmd.Play("res://Sora/sfx/formchange.wav");
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
            SfxCmd.Play("res://Sora/sfx/formchange.wav");

            var sonicBlade =
                base.CombatState.CreateCard<SonicBlade>(Owner);

            await CardCmd.AutoPlay(
                choiceContext,
                sonicBlade,
                null);
        }
    }
}