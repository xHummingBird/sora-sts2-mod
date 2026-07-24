using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sora.SoraCode.Cards.Rare;

public class LuckyEmblem() : SoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string _increaseKey = "Increase";

    private int _currentBlock = 5;

    private int _increasedBlock;

    public override bool GainsBlock => true;

    [SavedProperty]
    public int CurrentBlock
    {
        get { return _currentBlock; }
        set
        {
            AssertMutable();
            _currentBlock = value;
            base.DynamicVars.Block.BaseValue = _currentBlock;
        }
    }
    
    [SavedProperty]
    public int IncreasedBlock
    {
        get
        {
            return _increasedBlock;
        }
        set
        {
            AssertMutable();
            _increasedBlock = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(CurrentBlock, ValueProp.Move),
        new IntVar("Increase", 2m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        int intValue = base.DynamicVars["Increase"].IntValue;
        BuffFromPlay(intValue);
        (base.DeckVersion as LuckyEmblem)?.BuffFromPlay(intValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Increase"].UpgradeValueBy(1m);
    }

    protected override void AfterDowngraded()
    {
        UpdateBlock();
    }

    private void BuffFromPlay(int extraBlock)
    {
        IncreasedBlock += extraBlock;
        UpdateBlock();
    }

    private void UpdateBlock()
    {
        CurrentBlock = 5 + IncreasedBlock;
    }
}