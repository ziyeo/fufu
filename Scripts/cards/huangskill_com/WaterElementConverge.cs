using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class WaterElementConverge : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/water_element_converge.png";

    public WaterElementConverge() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) 
            return;

        // 获得元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,
            creature, 
            IsUpgraded ? 5 : 3,
            creature, 
            this
        );

        // 荒属性：沙龙水伤 +1，持续1回合
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,creature, 1, creature, this);
            if (buff != null)
            {
                buff.Duration = 1;
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：3元素 → 5元素
       base.OnUpgrade();
    }
   // 2.水元素汇聚：耗 1 点卡牌能量，获 3 元素能量；角色荒属性时沙龙水伤 + 1（1 回合）；升级5元素能量
}