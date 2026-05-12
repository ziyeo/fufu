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

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class BasicDrawCards : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/basic_draw_cards.png";

    public BasicDrawCards() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;
        int currentElemental = creature.GetPowerAmount<ElementEnergyPower>();
        int currentAtmosphere = creature.GetPowerAmount<AtmospherePower>();

        if (currentElemental < 5 || currentAtmosphere < 2)
        {
            return;
        }

        bool hasElementMastery = creature.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext, creature, -5, creature, this);
        }
        await PowerCmd.Apply<AtmospherePower>(choiceContext, creature, -2, creature, this);

        // 获得能量 + 抽牌（官方标准写法）
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}