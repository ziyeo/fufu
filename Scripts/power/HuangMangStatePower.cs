using BaseLib.Abstracts;
using Fufu.FufuModCode.Character;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;

namespace Fufu.Scripts.Power;

public class HuangMangStatePower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath
    {
        get
        {
            string iconName = FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang ? "huang_state" : "mang_state";
            return $"res://fufu/images/powers/{iconName}.png";
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            string iconName = FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang ? "huang_state" : "mang_state";
            return $"res://fufu/images/powers/big/{iconName}.png";
        }
    }
}