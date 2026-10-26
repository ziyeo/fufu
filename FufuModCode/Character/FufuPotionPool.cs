using BaseLib.Abstracts;
using Fufu.FufuModCode.Character;
using Godot;

namespace Fufu.FufuModCode.Character.PotionPool;

public class FufuPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => FufuMod.Color;

    // ✅ 绝对正确路径，不用扩展方法，不报错
    public override string BigEnergyIconPath => "res://fufu/images/charui/bigenergy.png";
    public override string TextEnergyIconPath => "res://fufu/images/charui/energy.png";
}