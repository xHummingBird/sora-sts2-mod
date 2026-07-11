using BaseLib.Abstracts;
using Sora.SoraCode.Extensions;
using Godot;

namespace Sora.SoraCode.Character;

public class SoraCardPool : CustomCardPoolModel
{
    public override string Title => Sora.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy_2.png".ImagePath();

    
    public override float H => 0.04f; //Hue; changes the color.
    public override float S => 0.75f; //Saturation
    public override float V => 0.82f; //Brightness
    
    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}