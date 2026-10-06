using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public class SituationCommand() : SoraCard(
    1,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.AnyEnemy),
    ISituationCard
{
    private const int SituationCommandCost = 30;

    protected override bool IsPlayable =>
        base.Owner.HasPower<SituationReadyPower>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    private IEnumerable<CardModel> GetUltimateFormCards()
    {
        var hand =
            PileType.Hand.GetPile(base.Owner);

        return hand.Cards
            .OfType<UltimateForm>();
    }

    private IEnumerable<CardModel> GetSituationCommandCards()
    {
        var hand =
            PileType.Hand.GetPile(base.Owner);

        return hand.Cards
            .OfType<SituationCommand>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SituationRelicBase? relic =
            Owner.Relics
                .OfType<SituationRelicBase>()
                .FirstOrDefault();

        if (relic == null)
            return;

        /*
         * Situation Commands require 30 SP.
         *
         * SituationReadyPower should normally guarantee
         * this, but this prevents forced card-play effects
         * from bypassing the SP requirement.
         */
        if (!relic.HasSituationPoints(
                SituationCommandCost))
        {
            return;
        }

        Wayfinder? wayfinder =
            base.Owner.GetRelic<Wayfinder>();

        bool hasRiku =
            Owner.Creature.HasPower<RikuPower>();

        bool hasKairi =
            Owner.Creature.HasPower<KairiPower>();

        List<CardModel> availableCommands =
            CreateAvailableCommands(
                hasRiku,
                hasKairi);

        /*
         * Wayfinder upgrades the available commands.
         *
         * SituationCommand itself is already made free
         * and upgraded by SituationReadyPower.
         */
        if (wayfinder != null)
        {
            foreach (CardModel command in availableCommands)
            {
                if (!command.IsUpgraded)
                {
                    CardCmd.Upgrade(command);
                }
            }
        }

        CardModel selectedCommand =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                availableCommands,
                Owner,
                canSkip: false);

        /*
         * The Situation Command has now been committed.
         *
         * Remove the Ready power so subsequent relic-based
         * SP gains are no longer doubled.
         */
        await PowerCmd.Remove<SituationReadyPower>(
            Owner.Creature);

        /*
         * Using a standard Situation Command spends 30 SP.
         *
         * This makes any generated Ultimate Form cards
         * invalid because Ultimate Form requires 60 SP.
         */
        foreach (CardModel card in
                 GetUltimateFormCards().ToList())
        {
            await CardCmd.Exhaust(
                choiceContext,
                card);
        }

        /*
         * Exhaust any other SituationCommand cards still
         * in hand. The currently played card should already
         * be resolving outside the hand.
         */
        foreach (CardModel card in
                 GetSituationCommandCards().ToList())
        {
            await CardCmd.Exhaust(
                choiceContext,
                card);
        }

        /*
         * Prevent SituationReadyPower from being reapplied
         * during the remainder of this turn.
         */
        relic.MarkSituationReadyConsumedThisTurn();

        /*
         * Every standard Situation Command costs 30 SP.
         */
        relic.SpendSituationPoints(
            SituationCommandCost);

        SfxCmd.Play(
            "res://Sora/sfx/formchange.wav");

        /*
         * Sonic Blade does not use the selected target.
         * All other commands receive the enemy selected
         * when SituationCommand was played.
         */
        await CardCmd.AutoPlay(
            choiceContext,
            selectedCommand,
            selectedCommand is SonicBlade
                ? null
                : play.Target);
    }

    private List<CardModel> CreateAvailableCommands(
        bool hasRiku,
        bool hasKairi)
    {
        List<CardModel> commands = [];

        /*
         * First command:
         *
         * No Links        -> Sonic Blade
         * Kairi only      -> Sonic Blade
         * Riku only       -> Sonic Blade
         * Riku and Kairi  -> Riku-Kairi Limit
         */
        if (hasRiku && hasKairi)
        {
            commands.Add(
                base.CombatState
                    .CreateCard<RikuKairiLimit>(
                        Owner));
        }
        else
        {
            commands.Add(
                base.CombatState
                    .CreateCard<SonicBlade>(
                        Owner));
        }

        /*
         * Second command:
         *
         * No Links        -> Ars Arcanum
         * Kairi only      -> Kairi Limit
         * Riku only       -> Riku Limit
         * Riku and Kairi  -> Riku Limit
         *
         * When both Links are active, Riku-Kairi Limit
         * replaces Sonic Blade while Riku Limit remains
         * available as the alternative command.
         */
        if (hasRiku)
        {
            commands.Add(
                base.CombatState
                    .CreateCard<RikuLimit>(
                        Owner));
        }
        else if (hasKairi)
        {
            commands.Add(
                base.CombatState
                    .CreateCard<KairiLimit>(
                        Owner));
        }
        else
        {
            commands.Add(
                base.CombatState
                    .CreateCard<ArsArcanum>(
                        Owner));
        }

        return commands;
    }
}