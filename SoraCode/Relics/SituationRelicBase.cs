using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Sora.SoraCode.Mechanics.SituationCommand;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Relics;

public abstract class SituationRelicBase : SoraRelic
{
    private int _situationPoints;

    /*
     * This means:
     * SituationCommand was used this turn,
     * so SituationReadyPower cannot be re-applied until next turn.
     *
     * UltimateForm and UltimateFinisher ignore this.
     */
    private bool _situationReadyConsumedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public int SituationPoints => _situationPoints;

    public abstract int MaxSituationPoints { get; }

    protected virtual int AttackSpGain => 2;

    protected virtual int TurnSpGain => 2;

    protected virtual bool CanGenerateUltimateForm => false;

    protected virtual bool IgnoreRelicBecauseBetterVersionExists => false;

    protected int SonicThreshold => 30;

    protected int FinisherThreshold => 60;

    protected int UltimateFormThreshold => 90;

    protected int UltimateFinisherThreshold => 30;

    public bool SonicBladeUnlocked => SituationPoints >= SonicThreshold;

    public bool FinisherUnlocked => SituationPoints >= FinisherThreshold;

    public bool UltimateFormUnlocked =>
        CanGenerateUltimateForm &&
        SituationPoints >= UltimateFormThreshold;

    public override int DisplayAmount => SituationPoints;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AttackSpGain", AttackSpGain),
        new DynamicVar("TurnSpGain", TurnSpGain),
        new DynamicVar("SonicThreshold", SonicThreshold),
        new DynamicVar("FinisherThreshold", FinisherThreshold),
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

            _situationPoints = Math.Clamp(value, 0, MaxSituationPoints);

            UpdateDisplay();
        }
    }

    public override Task BeforeCombatStart()
    {
        SituationPointsInternal = 0;
        _situationReadyConsumedThisTurn = false;
        base.Status = RelicStatus.Normal;

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        SituationPointsInternal = 0;
        _situationReadyConsumedThisTurn = false;
        base.Status = RelicStatus.Normal;

        return Task.CompletedTask;
    }
    
    public void MarkSituationReadyConsumedThisTurn()
    {
        AssertMutable();
        _situationReadyConsumedThisTurn = true;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (IgnoreRelicBecauseBetterVersionExists)
            return;

        CardModel card = cardPlay.Card;

        
        if (ShouldGainSpFromCard(card))
        {
            GainSituationPoints(AttackSpGain);
        }
        
        await CheckSituationCommands(
            choiceContext,
            base.Owner.Creature,
            card
        );
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
        
        _situationReadyConsumedThisTurn = false;

        GainSituationPoints(TurnSpGain);

        await CheckSituationCommands(
            null,
            base.Owner.Creature,
            null
        );
    }

    private bool ShouldGainSpFromCard(CardModel card)
    {
        if (card.Owner != base.Owner)
            return false;

        if (!CombatManager.Instance.IsInProgress)
            return false;
        
        if (card is ISituationCard)
            return false;

        if (card.Type != CardType.Attack)
            return false;

        return true;
    }

    private async Task CheckSituationCommands(
        PlayerChoiceContext? choiceContext,
        Creature source,
        CardModel? card)
    {
        Creature creature = base.Owner.Creature;
        
        if (creature.HasPower<UltimateFormPower>())
        {
            if (SituationPoints >= UltimateFinisherThreshold)
            {
                await EnsureUltimateFinisherExists(choiceContext);
            }

            return;
        }
        
        if (SituationPoints >= SonicThreshold &&
            SituationPoints <= UltimateFormThreshold &&
            !_situationReadyConsumedThisTurn &&
            !creature.HasPower<SituationReadyPower>())
        {
            await PowerCmd.Apply<SituationReadyPower>(
                choiceContext,
                creature,
                1,
                source,
                card
            );
        }
        
        if (CanGenerateUltimateForm &&
            SituationPoints >= UltimateFormThreshold)
        {
            await EnsureUltimateFormExists(choiceContext);
        }
    }

    private async Task EnsureUltimateFormExists(PlayerChoiceContext? choiceContext)
    {
        if (!CanGenerateUltimateForm)
            return;

        await EnsureGeneratedCardInHand<UltimateForm>(choiceContext);
    }

    private async Task EnsureUltimateFinisherExists(PlayerChoiceContext? choiceContext)
    {
        await EnsureGeneratedCardInHand<UltimateFinisher>(choiceContext);
    }

    private async Task EnsureGeneratedCardInHand<TCard>(PlayerChoiceContext? choiceContext)
        where TCard : CardModel
    {
        
        if (HasCardInHand<TCard>())
            return;
        
        var existing = FindCardOutsideHand<TCard>();

        if (existing != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(
                existing,
                PileType.Hand,
                base.Owner
            );

            return;
        }

        /*
         * If it truly does not exist anywhere,
         * create a new generated card.
         */
        var newCard = base.Owner.Creature.CombatState.CreateCard<TCard>(base.Owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            newCard,
            PileType.Hand,
            base.Owner
        );
    }

    private bool HasCardInHand<TCard>()
        where TCard : CardModel
    {
        var playerState = base.Owner.Creature.Player.PlayerCombatState;

        return playerState.AllCards
            .OfType<TCard>()
            .Any(c => c.Pile?.Type == PileType.Hand);
    }

    private TCard? FindCardOutsideHand<TCard>()
        where TCard : CardModel
    {
        var playerState = base.Owner.Creature.Player.PlayerCombatState;

        return playerState.AllCards
            .OfType<TCard>()
            .FirstOrDefault(c => c.Pile == null || c.Pile.Type != PileType.Hand);
    }

    public void GainSituationPoints(int amount)
    {
        if (amount <= 0)
            return;

        SituationPointsInternal += amount;
    }

    public void ConsumeSituationPoints(int amount)
    {
        if (amount <= 0)
            return;

        SituationPointsInternal -= amount;
    }

    public void ConsumeAllSituationPoints()
    {
        SituationPointsInternal = 0;
    }

    public void SetSituationPoints(int amount)
    {
        SituationPointsInternal = amount;
    }

    public int GetSituationPointsForUI()
    {
        return SituationPoints;
    }

    public int GetMaxSituationPointsForUI()
    {
        return MaxSituationPoints;
    }

    public int GetArrowProgressForUI()
    {
        if (SituationPoints < SonicThreshold)
            return SituationPoints;

        if (SituationPoints < FinisherThreshold)
            return SituationPoints - SonicThreshold;

        if (!CanGenerateUltimateForm)
            return 30;

        if (SituationPoints < UltimateFormThreshold)
            return SituationPoints - FinisherThreshold;

        return 30;
    }

    public bool IsSonicBladeUnlockedForUI()
    {
        return SonicBladeUnlocked;
    }

    public bool IsFinisherUnlockedForUI()
    {
        return FinisherUnlocked;
    }

    public bool IsUltimateFormUnlockedForUI()
    {
        return UltimateFormUnlocked;
    }

    public bool CanGenerateUltimateFormForUI()
    {
        return CanGenerateUltimateForm;
    }

    private void UpdateDisplay()
    {
        if (UltimateFormUnlocked || FinisherUnlocked || SonicBladeUnlocked)
        {
            base.Status = RelicStatus.Active;
        }
        else
        {
            base.Status = RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }
    
    public bool HasSituationPoints(int amount)
    {
        return SituationPoints >= amount;
    }

    public void SpendSituationPoints(int amount)
    {
        ConsumeSituationPoints(amount);
    }
    
    public int SpendAllSituationPoints()
    {
        int spent = SituationPoints;

        ConsumeAllSituationPoints();

        return spent;
    }
}