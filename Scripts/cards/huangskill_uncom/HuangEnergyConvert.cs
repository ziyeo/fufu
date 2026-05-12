using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class HuangEnergyConvert : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_energy_convert.png";

    public HuangEnergyConvert() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        // 基础：获得 10 元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, 10, player, this);

        // 荒属性：解锁 5元素 ↔ 抽1牌（施加被动能力）
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int element = player.GetPowerAmount<ElementEnergyPower>();
            int drawCount = element / 5;
            if (element >= 5)
            {
            // 扣除5元素
            bool hasElementMastery = player.HasPower<ElementMasteryPower>();   
            // 没有掌控 → 正常扣费
            if (!hasElementMastery)
            {
                await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -(drawCount * 5), player, this);
            }
            // 抽1张牌
            await CardPileCmd.Draw(choiceContext, drawCount, player.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：2费 → 1费
        base.EnergyCost.UpgradeBy(-1);
    }
}