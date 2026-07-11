using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class DamageSyphon() : SoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SituationReadyPower>(),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DamageSyphonPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}