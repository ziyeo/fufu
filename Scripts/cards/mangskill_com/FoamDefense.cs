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
public class FoamDefense : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7,ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/foam_defense.png";

    public FoamDefense() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 基础格挡
        int baseBlock = DynamicVars.Block.IntValue;

        // 芒属性：2元素 → 2格挡（资源互转）
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int currentElement = creature.GetPowerAmount<ElementEnergyPower>();          
            baseBlock += currentElement;
            bool hasElementMastery = creature.HasPower<ElementMasteryPower>();
    
            // 没有掌控 → 正常扣费
            if (!hasElementMastery)
            {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, -currentElement, creature, this);
            }
        }
        // **使用你提供的官方标准写法**
        await CreatureCmd.GainBlock(
            creature, 
            baseBlock, 
            DynamicVars.Block.Props, 
            cardPlay
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：7格挡 → 10格挡
        DynamicVars.Block.UpgradeValueBy(3);
    }

   
        //"获得 [Block] 点格挡。\n芒属性时：每有1元素能量，额外获得1点格挡。\n升级：格挡变为10。";
}