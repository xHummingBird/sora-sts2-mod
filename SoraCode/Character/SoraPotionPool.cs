using BaseLib.Abstracts;
using Sora.SoraCode.Extensions;
using Godot;

namespace Sora.SoraCode.Character;

public class SoraPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Sora.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}