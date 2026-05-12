using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;

namespace Fufu.FufuModCode.Powers;

public abstract class FufuModPower : CustomPowerModel
{
    // 效果图标（小图）
    public override string CustomPackedIconPath
    {
        get
        {
            string path = $"res://fufu/images/powers/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            return ResourceLoader.Exists(path) ? path : "res://fufu/images/powers/power.png";
        }
    }

    // 效果大图
    public override string CustomBigIconPath
    {
        get
        {
            string path = $"res://fufu/images/powers/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            return ResourceLoader.Exists(path) ? path : "res://fufu/images/powers/big/power.png";
        }
    }
}