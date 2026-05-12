using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using System.Linq;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class HuangOriginPower : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 标记为消耗牌
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_origin_power.png";

    public HuangOriginPower() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;

        // 本场战斗每损失1点生命 → 获得1元素
        int lostHp = player.MaxHp - player.CurrentHp;
        if (lostHp > 0)
        {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, lostHp, player, this);
        }

        // 荒属性时：获得5气氛值
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, 5, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

    //8.荒能本源（消耗）：耗 1 点卡牌能量，本场战斗每扣1血则 + 1 元素；荒属性时+5气氛值。升级去除消耗。
}