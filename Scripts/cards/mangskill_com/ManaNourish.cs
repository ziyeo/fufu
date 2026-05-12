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
public class ManaNourish : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mana_nourish.png";

    public ManaNourish() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 召唤歌者 2 回合
        await PowerCmd.Apply<BasicSingerPower>(choiceContext,creature, 2, creature, this);

        // 芒属性时：获得2元素 + 1气氛 + 抽牌
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            // 获得2元素
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, 2, creature, this);
            // 获得1气氛
            await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, 1, creature, this);

            // 抽牌（升级2张，基础1张）
            int drawCount = IsUpgraded ? 2 : DynamicVars.Cards.IntValue;
            await CardPileCmd.Draw(choiceContext, drawCount, player);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
     // "召唤歌者（2回合）。\n芒属性时：获得2元素能量、1气氛值，抽1张牌。\n升级：抽2张牌。";
}