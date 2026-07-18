using MegaCrit.Sts2.Core.Entities.Relics;

namespace Sora.SoraCode.Relics;

public class KingdomKey() : SituationRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override int MaxSituationPoints => 60;

    protected override int AttackSpGain => 3;

    protected override int TurnSpGain => 2;

    protected override bool CanGenerateUltimateForm => false;

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