using MegaCrit.Sts2.Core.Entities.Relics;

namespace Sora.SoraCode.Relics;

public class KingdomKey : SituationRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    protected override int AttackSpGain => 2;

    protected override int TurnSpGain => 1;

    protected override bool IgnoreRelicBecauseBetterVersionExists
    {
        get
        {
            UltimaWeapon? ultimaWeapon =
                base.Owner?.GetRelic<UltimaWeapon>();

            return ultimaWeapon != null;
        }
    }
}