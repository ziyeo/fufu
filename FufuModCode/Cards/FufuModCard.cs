using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace FufuMod.FufuModCode.Cards;

[Pool(typeof(FufuCardPool))]
public abstract class FufuModCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{// 大图（卡牌详情界面）
    public override string CustomPortraitPath => 
        $"res://fufu/images/card_portraits/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    // 小图（战斗/卡组界面）
    public override string PortraitPath => 
        $"res://fufu/images/card_portraits/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    // Beta 图
    public override string BetaPortraitPath => 
        $"res://fufu/images/card_portraits/beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
}