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
public class SoftHealingChant : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(4),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/soft_healing_chant.png";

    public SoftHealingChant() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 基础治疗
         await CreatureCmd.Heal(creature, DynamicVars.Heal.IntValue);
        // 芒属性额外+2
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
             // 获得2气氛值
            await CreatureCmd.Heal(creature,2);
            await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, 2, creature, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
        }     
       
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
       // "恢复4点生命。\n芒属性时治疗+2。\n获得2点气氛值，抽1张牌。\n升级：抽2张牌。";
}