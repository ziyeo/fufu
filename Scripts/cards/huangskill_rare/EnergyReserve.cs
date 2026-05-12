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
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class EnergyReserve : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };

    public override string PortraitPath => "res://fufu/images/card_portraits/energy_reserve.png";

    public EnergyReserve() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) 
            return;

        Player player = creature.Player;

        // 检查元素能量是否 ≥5
        int element = creature.GetPowerAmount<ElementEnergyPower>();
        if (element < 5)
            return;

        // 扣除 5 元素
        int energyToGain = element/5;
        bool hasElementMastery = creature.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, -5*energyToGain, creature, this);
        }

        // 获得 1 点能量
        await PlayerCmd.GainEnergy(energyToGain, player);
    }

    protected override void OnUpgrade()
    {
        // 升级：费用 1 → 0
        base.EnergyCost.UpgradeBy(-1);
    }
}