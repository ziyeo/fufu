using BaseLib.Abstracts;
using Godot;

namespace Fufu.FufuModCode.Character.CardPool;

public class FufuCardPool : CustomCardPoolModel
{
    public override string Title => "Fufu";

    // 能量图标路径（正确）
    public override string BigEnergyIconPath => "res://fufu/images/charui/bigenergy.png";
    public override string TextEnergyIconPath => "res://fufu/images/charui/energy.png";
    // 蓝色能量（直接生效）
    public override float H => 0.54f;
    public override float S => 0.62f;
    public override float V => 0.98f;

    public override Color DeckEntryCardColor => new Color("#73d9ff");

    public override bool IsColorless => false;
}