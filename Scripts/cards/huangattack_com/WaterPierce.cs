using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.ValueProps;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.Models.Powers;


namespace Fufu.Scripts.Cards;
[Pool(typeof(FufuCardPool))]
public sealed class WaterPierce : CustomCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
     protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m)
    };
    public override string PortraitPath => "res://fufu/images/card_portraits/water_pierce.png";
    // 构造：1费 攻击 白卡 目标敌人
    public WaterPierce(): base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        Creature player = Owner.Creature;

        // 1. 造成基础伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 荒属性 + 元素能量 ≥ 5
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang && player.GetPowerAmount<ElementEnergyPower>() >= 5)
        {
            bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
            // 没有掌控 → 正常扣费
            if (!hasElementMastery)
            {
            // 扣除5元素能量
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -5, player, this);
            }

            // 施加易伤（官方写法）
            await PowerCmd.Apply<VulnerablePower>(choiceContext,cardPlay.Target!, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }
    }

    // 升级：+3伤害 +1易伤
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}