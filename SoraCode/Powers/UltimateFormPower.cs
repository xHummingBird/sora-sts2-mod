using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Cards.Ancient;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Powers;

// Duration buff. Amount stores the number of turns remaining.
// Decrements at the end of each of your turns and is removed
// when it reaches 0.
public class UltimateFormPower : SoraPower
{
    private const int UltimateFinisherThreshold = 30;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(
            "DamageIncrease",
            1.50m)
    ];

    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool AllowNegative =>
        false;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack())
            return 1m;

        if (dealer != base.Owner)
            return 1m;

        UltimaWeapon? ultimaWeapon =
            base.Owner.Player.GetRelic<UltimaWeapon>();

        return ultimaWeapon != null
            ? 1.75m
            : 1.50m;
    }

    /*
     * Run after every card resolves so Ultimate Finisher
     * can be generated immediately when that card causes
     * the Ultimate Form gauge to reach 30 SP.
     */
    
    public async Task CheckUltimateFinisher(
        PlayerChoiceContext choiceContext)
    {
        SituationRelicBase? relic =
            base.Owner.Player.GetRelic<SituationRelicBase>();

        if (relic == null)
            return;

        if (relic.SituationPoints < UltimateFinisherThreshold)
            return;

        await EnsureUltimateFinisherAvailable(
            choiceContext,
            allowInitialGeneration: true);
    }

    /*
     * At the start of Sora's turn, return an existing
     * Ultimate Finisher to hand if it was accidentally
     * discarded or exhausted.
     *
     * This does not create a replacement if an existing
     * Ultimate Finisher cannot be found.
     */
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Side)
            return;

        SituationRelicBase? relic =
            base.Owner.Player.GetRelic<SituationRelicBase>();

        if (relic == null)
            return;

        if (relic.SituationPoints < UltimateFinisherThreshold)
            return;

        await EnsureUltimateFinisherAvailable(
            new ThrowingPlayerChoiceContext(),
            allowInitialGeneration: false);
    }

    private async Task EnsureUltimateFinisherAvailable(
        PlayerChoiceContext? choiceContext,
        bool allowInitialGeneration)
    {
        var player =
            base.Owner.Player;

        var playerState =
            player.PlayerCombatState;

        if (playerState == null)
            return;

        /*
         * Do nothing if Ultimate Finisher is already
         * available in hand.
         */
        bool isAlreadyInHand =
            playerState.AllCards
                .OfType<UltimateFinisher>()
                .Any(card =>
                    card.Pile?.Type == PileType.Hand);

        if (isAlreadyInHand)
            return;

        /*
         * Look for existing copies outside the hand.
         *
         * Materialize the result before passing it to
         * CardPileCmd because moving cards changes their
         * pile while the operation runs.
         */
        List<UltimateFinisher> existingFinishers =
            playerState.AllCards
                .OfType<UltimateFinisher>()
                .Where(card =>
                    card.Pile == null ||
                    card.Pile.Type != PileType.Hand)
                .ToList();

        /*
         * If a copy already exists, retrieve it instead
         * of generating a replacement.
         */
        if (existingFinishers.Count > 0)
        {
            await CardPileCmd.Add(
                existingFinishers,
                PileType.Hand);

            return;
        }

        /*
         * Only the post-card-play check may generate the
         * first Ultimate Finisher.
         *
         * The turn-start check only retrieves an existing
         * copy.
         */
        if (!allowInitialGeneration)
            return;

        UltimateFinisher ultimateFinisher =
            base.Owner.CombatState
                .CreateCard<UltimateFinisher>(
                    player);

        /*
         * Ultima Weapon upgrades Ultimate Finisher.
         */
        if (player.GetRelic<UltimaWeapon>() != null &&
            !ultimateFinisher.IsUpgraded)
        {
            CardCmd.Upgrade(
                ultimateFinisher);
        }

        await CardPileCmd.AddGeneratedCardToCombat(
            ultimateFinisher,
            PileType.Hand,
            player);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != base.Owner.Side)
            return;

        bool formWillExpire =
            Amount <= 1;

        if (formWillExpire)
        {
            /*
             * Ultimate Finisher becomes invalid when
             * Ultimate Form expires, so exhaust every
             * remaining copy currently in hand.
             */
            await ExhaustUltimateFinishersInHand(
                choiceContext);
        }

        await PowerCmd.Decrement(
            this);

        if (!formWillExpire)
            return;

        Creature ownerCreature =
            base.Owner;

        if (base.Owner.Player.Character is Character.Sora sora)
        {
            sora.PlayAnimation(
                ownerCreature,
                "idle_normal");
        }
    }

    private async Task ExhaustUltimateFinishersInHand(
        PlayerChoiceContext choiceContext)
    {
        var playerState =
            base.Owner.Player.PlayerCombatState;

        if (playerState == null)
            return;

        List<UltimateFinisher> finishersInHand =
            playerState.AllCards
                .OfType<UltimateFinisher>()
                .Where(card =>
                    card.Pile?.Type == PileType.Hand)
                .ToList();

        foreach (UltimateFinisher finisher in finishersInHand)
        {
            await CardCmd.Exhaust(
                choiceContext,
                finisher);
        }
    }
}