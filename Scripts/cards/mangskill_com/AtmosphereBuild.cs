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

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class AtmosphereBuild : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };
    public override string PortraitPath => "res://fufu/images/card_portraits/atmosphere_build.png";

    public AtmosphereBuild() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 基础气氛值
        int baseAtmos = 5;

        // 升级额外 +2 气氛
        if (IsUpgraded)
        {
            baseAtmos += 2;
        }

        // 获得基础气氛
        await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, baseAtmos, creature, this);

        // 芒属性：额外 +2 元素
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, 2, creature, this);
            // 资源互转：1 元素 → 2 气氛
            int currentElement = creature.GetPowerAmount<ElementEnergyPower>();
            if (currentElement >= 0)
            {
                bool hasElementMastery = creature.HasPower<ElementMasteryPower>();  
                // 没有掌控 → 正常扣费
                if (!hasElementMastery)
                {
                await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, -currentElement, creature, this);
                }
                await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, 2*currentElement, creature, this);
            }
        }        
    }

    protected override void OnUpgrade()
    {
        // 升级效果已在 OnPlay 中实现
    }

     //   "获得 5 点气氛值。\n芒属性时额外获得2元素能量。资源互转：1元素→2气氛值。\n升级：额外获得+2气氛值。";
}