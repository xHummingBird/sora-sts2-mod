using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Sora.SoraCode.Extensions;
using Sora.SoraCode.Powers;

namespace Sora.SoraCode.Cards.Rare;

public class HeartRecovery() : SoraCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    private const int LinkTurns = 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<KairiPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        AudioHelper.PlayRandomFormchange();

        await SoraExtensions.CombatHelpers.RefreshLink<KairiPower>(
            choiceContext,
            base.Owner.Creature,
            this,
            LinkTurns);

        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards.ToList())
        {
            if (card.Type == CardType.Skill && !card.EnergyCost.CostsX)
            {
                card.SetToFreeThisTurn();
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
