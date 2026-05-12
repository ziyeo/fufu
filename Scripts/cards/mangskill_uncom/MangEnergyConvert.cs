using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class MangEnergyConvert : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 这张牌是【消耗牌】
   public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };
    public override string PortraitPath => "res://fufu/images/card_portraits/mang_energy_convert.png";

    public MangEnergyConvert() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;
        Player players = player.Player;

        // 基础效果：获得 4 元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, IsUpgraded ? 7 : 4, player, this);

        // 芒属性：解锁 5气氛 ↔ 抽1牌 被动
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {          
            int atmosphere = player.GetPowerAmount<AtmospherePower>();
            int drawTimes = atmosphere / 5;

            if (drawTimes > 0)
            {
            // 扣除气氛
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -drawTimes * 5, player, this);
            // 抽牌
            await CardPileCmd.Draw(choiceContext, drawTimes, player.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：元素 +3 → 4→7
        base.OnUpgrade();
    }

    //4.芒能转化：耗 1 点卡牌能量，获 4 元素；角色芒属性时解锁 5 气氛↔抽1牌 ；（消耗），升级+3元素能量
}