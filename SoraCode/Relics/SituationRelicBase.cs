using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sora.SoraCode.Cards;
using Sora.SoraCode.Cards.Ancient;
using Sora.SoraCode.Mechanics.Companion;
using Sora.SoraCode.Mechanics.SituationCommand;
using Sora.SoraCode.Potions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Relics;

public abstract class SituationRelicBase : SoraRelic
{
    private int _situationPoints;

    /*
     * A standard Situation Command was used this turn,
     * so SituationReadyPower cannot be reapplied until
     * the next turn.
     *
     * Ultimate Form and Ultimate Finisher ignore this.
     */
    private bool _situationReadyConsumedThisTurn;

    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override bool ShowCounter =>
        CombatManager.Instance.IsInProgress;

    public override int DisplayAmount =>
        SituationPoints;

    public int SituationPoints =>
        _situationPoints;

    public int MaxSituationPoints => 60;
    protected virtual int AttackSpGain => 2;
    protected virtual int CompanionSpGain => 3;
    protected virtual int TurnSpGain => 1;
    protected virtual bool IgnoreRelicBecauseBetterVersionExists => false;
    protected int SituationCommandThreshold => 30;
    protected int UltimateFormThreshold => 60;
    protected int UltimateFinisherThreshold => 30;

    public bool SituationCommandsUnlocked => !base.Owner.Creature.HasPower<UltimateFormPower>() && SituationPoints >= SituationCommandThreshold;

    public bool UltimateFormUnlocked => !base.Owner.Creature.HasPower<UltimateFormPower>() && SituationPoints >= UltimateFormThreshold;

    public bool UltimateFinisherUnlocked => base.Owner.Creature.HasPower<UltimateFormPower>() && SituationPoints >= UltimateFinisherThreshold;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AttackSpGain", AttackSpGain),
        new DynamicVar("CompanionSpGain", CompanionSpGain),
        new DynamicVar("TurnSpGain", TurnSpGain),
        new DynamicVar("SituationCommandThreshold", SituationCommandThreshold),
        new DynamicVar("UltimateFormThreshold", UltimateFormThreshold),
        new DynamicVar("UltimateFinisherThreshold", UltimateFinisherThreshold),
        new DynamicVar("MaxSp", MaxSituationPoints)
    ];

    private int SituationPointsInternal
    {
        get => _situationPoints;

        set
        {
            AssertMutable();

            /*
             * Ultimate Form uses a separate 30-SP gauge.
             */
            int maximum =
                base.Owner.Creature.HasPower<UltimateFormPower>()
                    ? UltimateFinisherThreshold
                    : MaxSituationPoints;

            _situationPoints =
                Math.Clamp(
                    value,
                    0,
                    maximum);

            UpdateDisplay();
        }
    }

    public override Task BeforeCombatStart()
    {
        SituationPointsInternal = 0;
        _situationReadyConsumedThisTurn = false;

        base.Status =
            RelicStatus.Normal;

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(
        CombatRoom _)
    {
        SituationPointsInternal = 0;
        _situationReadyConsumedThisTurn = false;

        base.Status =
            RelicStatus.Normal;

        return Task.CompletedTask;
    }

    public void MarkSituationReadyConsumedThisTurn()
    {
        AssertMutable();

        _situationReadyConsumedThisTurn = true;
    }

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if (potion is not PaopuFruit)
            return;

        if (potion.Owner != base.Owner)
            return;
        
        await CheckSituationUnlocks(
            new ThrowingPlayerChoiceContext(),
            base.Owner.Creature,
            null,
            canGenerateUltimateForm: true);
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (IgnoreRelicBecauseBetterVersionExists)
            return;

        CardModel card =
            cardPlay.Card;

        if (card.Owner != base.Owner)
            return;

        int relicSpGain =
            GetRelicSpGainFromCard(card);

        GainSituationPointsFromRelic(
            relicSpGain);

        /*
         * This runs after the played card has resolved and
         * after that card's relic-generated SP was awarded.
         */
        await CheckSituationUnlocks(
            choiceContext,
            base.Owner.Creature,
            card,
            canGenerateUltimateForm: true);
        
        UltimateFormPower? ultimateFormPower =
            base.Owner.Creature.GetPower<UltimateFormPower>();

        if (ultimateFormPower != null)
        {
            await ultimateFormPower.CheckUltimateFinisher(
                choiceContext);
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (IgnoreRelicBecauseBetterVersionExists)
            return;

        if (side != base.Owner.Creature.Side)
            return;

        /*
         * A new turn allows another standard
         * Situation Command to be used.
         */
        _situationReadyConsumedThisTurn = false;

        int turnSpGain =
            TurnSpGain;

        /*
         * Negative Combo reduces turn-start SP by 1.
         * With the current base gain, this reduces it
         * from 1 to 0.
         */
        if (base.Owner.Creature.HasPower<NegativeComboPower>())
        {
            turnSpGain -= 1;
        }

        GainSituationPointsFromRelic(
            turnSpGain);
        
        UltimateFormPower? ultimateFormPower =
            base.Owner.Creature
                .GetPower<UltimateFormPower>();

        if (ultimateFormPower != null)
        {
            await ultimateFormPower.CheckUltimateFinisher(
                new ThrowingPlayerChoiceContext());
        }
        
        await CheckSituationUnlocks(
            new ThrowingPlayerChoiceContext(),
            base.Owner.Creature,
            null,
            canGenerateUltimateForm: true);
    }

    private int GetRelicSpGainFromCard(
        CardModel card)
    {
        if (card.Owner != base.Owner)
            return 0;

        if (!CombatManager.Instance.IsInProgress)
            return 0;
        
        if (card is ISituationCard)
            return 0;

        int amount = 0;
        
        if (card is ICompanionCard)
        {
            amount += CompanionSpGain;
        }
        else if (card.Type == CardType.Attack)
        {
            amount += AttackSpGain;
        }
        
        return amount;
    }

    /*
     * SP managed by this relic is doubled while
     * SituationReadyPower is active.
     *
     * This includes:
     *
     * - Turn-start SP
     * - Attack SP
     * - Companion card SP
     */
    private void GainSituationPointsFromRelic(
        int amount)
    {
        if (amount <= 0)
            return;

        int bonus =
            base.Owner.Creature
                .GetPowerAmount<SituationBoostPower>();

        if (base.Owner.Creature.HasPower<SituationReadyPower>() ||
            base.Owner.Creature.HasPower<UltimateFormPower>())
        {
            amount *= 2;
        }

        SituationPointsInternal +=
            amount + bonus;
    }

    private async Task CheckSituationUnlocks(
        PlayerChoiceContext? choiceContext,
        Creature source,
        CardModel? card,
        bool canGenerateUltimateForm)
    {
        Creature creature =
            base.Owner.Creature;

        /*
         * Standard Situation Commands and Ultimate Form
         * are unavailable while Ultimate Form is active.
         *
         * UltimateFinisher generation is handled by
         * UltimateFormPower.
         */
        if (creature.HasPower<UltimateFormPower>())
            return;

        /*
         * At 30 SP, apply SituationReadyPower.
         *
         * After a standard Situation Command is used,
         * the consumed flag prevents this from returning
         * during the same turn.
         *
         * The flag resets at the start of the next turn,
         * allowing the power to be reapplied if the player
         * still has at least 30 SP.
         */
        if (SituationPoints >= SituationCommandThreshold &&
            !_situationReadyConsumedThisTurn &&
            !creature.HasPower<SituationReadyPower>())
        {
            await PowerCmd.Apply<SituationReadyPower>(
                choiceContext,
                creature,
                1,
                source,
                card);
        }

        /*
         * Generate the first Ultimate Form after a card
         * has finished resolving and the gauge has reached
         * 60 SP.
         *
         * SituationReadyPower is responsible for recovering
         * an Ultimate Form that is later discarded or
         * exhausted without being used.
         */
        if (canGenerateUltimateForm &&
            SituationPoints >= UltimateFormThreshold)
        {
            await GenerateInitialUltimateForm(
                choiceContext);
        }
    }

    private async Task GenerateInitialUltimateForm(
        PlayerChoiceContext? choiceContext)
    {
        var playerState =
            base.Owner.Creature.Player.PlayerCombatState;

        UltimateForm? existingForm =
            playerState.AllCards
                .OfType<UltimateForm>()
                .FirstOrDefault();

        if (existingForm != null)
        {
            if (existingForm.Pile?.Type != PileType.Hand)
            {
                await CardPileCmd.Add(
                    [existingForm],
                    PileType.Hand);
            }

            return;
        }

        UltimateForm ultimateForm =
            base.Owner.Creature.CombatState
                .CreateCard<UltimateForm>(base.Owner);

        if (base.Owner.GetRelic<UltimaWeapon>() != null &&
            !ultimateForm.IsUpgraded)
        {
            CardCmd.Upgrade(ultimateForm);
        }

        await CardPileCmd.AddGeneratedCardToCombat(
            ultimateForm,
            PileType.Hand,
            base.Owner);
    }

    /*
     * Public SP gain is intended for explicit card effects
     * and external sources.
     *
     * This method deliberately does not apply the
     * SituationReadyPower multiplier.
     */
    public void GainSituationPoints(
        int amount)
    {
        if (amount <= 0)
            return;

        SituationPointsInternal += amount;
    }

    public void ConsumeSituationPoints(
        int amount)
    {
        if (amount <= 0)
            return;

        SituationPointsInternal -= amount;
    }

    public void ConsumeAllSituationPoints()
    {
        SituationPointsInternal = 0;
    }

    public void SetSituationPoints(
        int amount)
    {
        SituationPointsInternal = amount;
    }

    public int GetSituationPointsForUI()
    {
        return SituationPoints;
    }

    public int GetMaxSituationPointsForUI()
    {
        if (base.Owner.Creature.HasPower<UltimateFormPower>())
        {
            return UltimateFinisherThreshold;
        }

        return MaxSituationPoints;
    }

    /*
     * Returns progress within the current 30-SP section.
     *
     * Normal:
     * 0-29 SP  -> 0-29
     * 30-60 SP -> 0-30
     *
     * Ultimate Form:
     * 0-30 SP  -> 0-30
     */
    public int GetArrowProgressForUI()
    {
        if (base.Owner.Creature.HasPower<UltimateFormPower>())
        {
            return Math.Clamp(
                SituationPoints,
                0,
                UltimateFinisherThreshold);
        }

        if (SituationPoints < SituationCommandThreshold)
        {
            return SituationPoints;
        }

        return Math.Clamp(
            SituationPoints - SituationCommandThreshold,
            0,
            UltimateFormThreshold -
            SituationCommandThreshold);
    }

    public bool AreSituationCommandsUnlockedForUI()
    {
        return SituationCommandsUnlocked;
    }

    public bool IsUltimateFormUnlockedForUI()
    {
        return UltimateFormUnlocked;
    }

    public bool IsUltimateFinisherUnlockedForUI()
    {
        return UltimateFinisherUnlocked;
    }

    private void UpdateDisplay()
    {
        bool commandAvailable =
            SituationCommandsUnlocked ||
            UltimateFormUnlocked ||
            UltimateFinisherUnlocked;

        base.Status =
            commandAvailable
                ? RelicStatus.Active
                : RelicStatus.Normal;

        InvokeDisplayAmountChanged();
    }

    public bool HasSituationPoints(
        int amount)
    {
        return SituationPoints >= amount;
    }

    public void SpendSituationPoints(
        int amount)
    {
        ConsumeSituationPoints(
            amount);
    }

    public int SpendAllSituationPoints()
    {
        int spent =
            SituationPoints;

        ConsumeAllSituationPoints();

        return spent;
    }
}