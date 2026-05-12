using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class MangSaintApotheosis : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10,ValueProp.Move),
        new HealVar(4)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_saint_apotheosis.png";

    public MangSaintApotheosis() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;

        // 基础效果
        await CreatureCmd.GainBlock(
            player, 
            DynamicVars.Block.BaseValue, 
            DynamicVars.Block.Props, 
            cardPlay);
        await CreatureCmd.Heal(player, DynamicVars.Heal.BaseValue);
        await PowerCmd.Apply<AtmospherePower>(choiceContext,player, 10, player, this);

        // 芒属性：召唤1名歌者
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await PowerCmd.Apply<BasicSingerPower>(choiceContext,player, 1, player, this);
        }

        // 元素能量判定
        int element = player.GetPowerAmount<ElementEnergyPower>();
        if (element < 6)
            return;

        // 扣除6元素
        bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -6, player, this);
        }

        // 抽2牌
        await CardPileCmd.Draw(choiceContext, 2, player.Player);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars.Heal.UpgradeValueBy(2);
    }

    //1.芒性圣化：耗2费，获10格挡，恢复4血，气氛+10；芒属性时召唤1名歌者；需元素≥6，打出扣6元素，抽2牌。升级+3格挡，+2回血。
}