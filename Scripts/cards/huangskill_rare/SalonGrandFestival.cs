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
public class SalonGrandFestival : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/salon_grand_festival.png";

    public SalonGrandFestival() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 召唤 2 名沙龙
        bool hasSalonBond = creature.HasPower<SalonBondPower>();
        int salonBond = creature.GetPowerAmount<SalonBondPower>();
        await PowerCmd.Apply<ShadowSalonPower>(choiceContext,creature, 2, creature, this);
        if(hasSalonBond)
            await CardPileCmd.Draw(choiceContext, salonBond, player);

        // 荒属性：每有 1 名沙龙，获得 3 元素
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int salonCount = creature.GetPowerAmount<ShadowSalonPower>();
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, salonCount * 3, creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：1费 → 0费
        base.EnergyCost.UpgradeBy(-1);
    }

    //2.沙龙盛典：耗 1 点卡牌能量，召唤 2 名沙龙；荒属性时每有 1 名沙龙获 3 元素。升级0能量
}