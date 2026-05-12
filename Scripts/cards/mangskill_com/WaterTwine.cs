using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.ValueProps;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class WaterTwine : CustomCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型（技能）
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VulnerablePower>(2m)
    };

    public override string PortraitPath => "res://fufu/images/card_portraits/water_twine.png";

    // 构造：1费 技能 白卡 目标敌人
    public WaterTwine() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        Creature player = Owner.Creature;

        // 基础易伤层数
        decimal vulnerableAmount = DynamicVars.Vulnerable.BaseValue;

        // 芒属性 → 易伤持续 +1
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            vulnerableAmount += 1;
            // 获得 1 点元素能量
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, 1, player, this);
        }

        // 施加易伤（官方标准写法）
        await PowerCmd.Apply<VulnerablePower>(choiceContext,
            cardPlay.Target!, 
            vulnerableAmount, 
            Owner.Creature, 
            this
        );
    }

    // 升级：易伤层数 +1
    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }

    
        // "对所有敌人施加 {VulnerablePower} 层易伤。\n" +
        // "芒属性时：易伤层数+1。\n" +
        // "获得1点元素能量。\n" +
        // "升级：易伤层数+1。";
}