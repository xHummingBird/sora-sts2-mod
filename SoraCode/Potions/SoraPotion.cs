using BaseLib.Abstracts;
using BaseLib.Utils;
using Sora.SoraCode.Character;

namespace Sora.SoraCode.Potions;

[Pool(typeof(SoraPotionPool))]
public abstract class SoraPotion : CustomPotionModel;