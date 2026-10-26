using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using Fufu.FufuModCode.Character;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class MangHolyStrike : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new CardsVar(1),
        new HealVar(5),
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_holy_strike.png";

    public MangHolyStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        if (cardPlay.Target == null) return;

        // 造成12点基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 恢复5点生命
        await CreatureCmd.Heal(creature, DynamicVars.Heal.BaseValue);

        // 芒属性：召唤歌者+2 + 获得6气氛
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await PowerCmd.Apply<BasicSingerPower>(choiceContext,creature, 2, creature, this);
            await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, 6, creature, this);
        }
         // 检查元素能量 ≥10
        int element = creature.GetPowerAmount<ElementEnergyPower>();
        if (element < 10) 
            return;

        // 扣除10元素能量
        bool hasElementMastery = creature.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, -10, creature, this);
        }

        // 抽牌
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
    }

    protected override void OnUpgrade()
    {
        // 升级：抽牌+1（1→2）
        DynamicVars.Cards.UpgradeValueBy(1);
    }

    //1.芒性圣击：耗 2 点卡牌能量，造 12 普通伤害，恢复 5血 ；芒属性时召唤歌者 + 2，获 6 气氛；需元素能量≥10，打出扣 10 元素能量，抽 1 牌。升级抽2牌
}