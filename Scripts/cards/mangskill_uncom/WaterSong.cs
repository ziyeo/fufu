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
public class WaterSong : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 动态变量：基础格挡3点
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3, ValueProp.Move),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/water_song.png";

    public WaterSong() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;
        Player players = player.Player;
        // 基础效果：歌者治疗 +2（1回合）
        var healBuff = await PowerCmd.Apply<SingerTempHealBuffPower>(choiceContext,player, 2, player, this);
        if (healBuff != null)
        {
            healBuff.Duration = 1;
        }
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {          
           await CreatureCmd.GainBlock(player, DynamicVars.Block.IntValue, DynamicVars.Block.Props, cardPlay);
        }
        // 检查元素能量 ≥10（必须满足才能触发后续所有效果）
        int element = player.GetPowerAmount<ElementEnergyPower>();
        if (element < 5) 
            return;

        // 扣除5元素能量
         bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -5, player, this);
        }

        // 获得3气氛
        await PowerCmd.Apply<AtmospherePower>(choiceContext,player, 3, player, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, players);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
    //2.众水之歌：耗 1 点卡牌能量，歌者治疗 + 2（1回合）；角色芒属性时自身回合获 3 格挡；需元素能量≥5，打出扣 5 元素能量，获 3 气氛值，抽 1 牌。升级+2格挡，+1抽牌
}