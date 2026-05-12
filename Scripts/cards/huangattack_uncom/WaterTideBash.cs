using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.ValueProps;
using Fufu.Scripts.Power;


namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class WaterTideBash : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12,ValueProp.Move),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/water_tide_bash.png";

    public WaterTideBash() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        if (cardPlay.Target is null) return;

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 1. 基础伤害 12 点
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);

        // 2. 荒属性判定
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            // 3. 检查元素 ≥5
        int currentElement = creature.GetPowerAmount<ElementEnergyPower>();
        if (currentElement < 5)
        {
            return;
        }

        // 4. 扣除 5 元素
        // 没有掌控 → 正常扣费
        bool hasElementMastery = creature.HasPower<ElementMasteryPower>();
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, -5, creature, this);
        }

        // 5. 抽牌（升级后 2 张）
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
        }

        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}