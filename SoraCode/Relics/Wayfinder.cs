using MegaCrit.Sts2.Core.Entities.Relics;

namespace Sora.SoraCode.Relics;

// Situation Commands generated while you hold this relic cost 0 and are
// upgraded. The actual effect is applied in SituationReadyPower, which creates
// the Situation Command cards.
public class Wayfinder : SoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
}
