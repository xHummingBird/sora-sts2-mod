using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Sora.SoraCode.Powers;

// At the start of your turn, return a random attack card from your discard
// pile to your hand.
public class PowerOfWakingPower : SoraPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        IReadOnlyList<CardModel> readOnlyList = base.Owner.Player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).Where(delegate(CardModel c)
        {
            CardRarity rarity = c.Rarity;
            bool flag = ((rarity == CardRarity.Basic || rarity == CardRarity.Ancient) ? true : false);
            return !flag;
        }).ToList();
        if (readOnlyList.Count > 0)
        {
            CardModel[] array = new CardModel[base.Amount];
            Rng combatCardGeneration = base.Owner.Player.RunState.Rng.CombatCardGeneration;
            Flash();
            await CardPileCmd.AddGeneratedCardsToCombat(array, PileType.Hand, base.Owner.Player);
        }
    }
}
