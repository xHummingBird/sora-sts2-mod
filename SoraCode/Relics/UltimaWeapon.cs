using MegaCrit.Sts2.Core.Entities.Relics;

namespace Sora.SoraCode.Relics;

public class UltimaWeapon() : SituationRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override int MaxSituationPoints => 90;

    protected override int AttackSpGain => 3;

    protected override int TurnSpGain => 3;

    protected override bool CanGenerateUltimateForm => true;
}