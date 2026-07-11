using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Sora.SoraCode.Extensions;

namespace Sora.SoraCode.Cards.Rare;

public class PowerOfWaking() : SoraCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(5),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        if (ownerCreature != null && Owner?.Character is Character.Sora sora)
        {
            // attack animation
            float duration = sora.PlayAnimation(ownerCreature, "cast").total;
            SfxCmd.Play("res://Sora/sounds/hikari.wav");
        }
        foreach (CardModel item in PileType.Hand.GetPile(base.Owner).Cards.ToList())
        {
            await CardPileCmd.Add(item, PileType.Draw);
        }
        await CardPileCmd.Shuffle(choiceContext, base.Owner);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}