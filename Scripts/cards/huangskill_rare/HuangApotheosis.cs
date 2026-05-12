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
public class HuangApotheosis : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_apotheosis.png";

    public HuangApotheosis() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 沙龙水伤 +2
        var buff = await PowerCmd.Apply<SalonDamageBuffPower>(choiceContext,creature, 2, creature, this);
        if (buff != null)
        {  }

        // 获得 6 元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, IsUpgraded ? 9 : 6, creature, this);

        // 荒属性：召唤 2 名沙龙
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            bool hasSalonBond = creature.HasPower<SalonBondPower>();
            int salonBond = creature.GetPowerAmount<SalonBondPower>();
            await PowerCmd.Apply<ShadowSalonPower>(choiceContext,creature, 2, creature, this);
            if(hasSalonBond)
                await CardPileCmd.Draw(choiceContext, salonBond, player);
        }
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    //1.荒性神化：耗 2点卡牌能量，沙龙水伤 +2 ，获 6 元素；荒属性时召唤 2 名沙龙；升级+3元素能量。
}