using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Fufu.FufuModCode.Character.RelicPool;
using Godot;

namespace Fufu.FufuModCode.Relics;

[Pool(typeof(FufuRelicPool))]
public abstract class FufuModRelic : CustomRelicModel
{
    // 遗物小图标
    public override string PackedIconPath
    {
        get
        {
            string path = $"res://fufu/images/relics/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            return ResourceLoader.Exists(path) ? path : "res://fufu/images/relics/relic.png";
        }
    }

    // 遗物外框
    // protected override string PackedIconOutlinePath
    // {
    //     get
    //     {
    //         string path = $"res://fufu/images/relics/{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";
    //         return ResourceLoader.Exists(path) ? path : "res://fufu/images/relics/relic_outline.png";
    //     }
    // }

    // 遗物大图
    protected override string BigIconPath
    {
        get
        {
            string path = $"res://fufu/images/relics/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            return ResourceLoader.Exists(path) ? path : "res://fufu/images/relics/big/relic.png";
        }
    }
}