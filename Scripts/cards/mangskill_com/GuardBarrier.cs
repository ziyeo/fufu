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
public class GuardBarrier : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move),
        new EnergyVar(2),
        new HealVar(2)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/guard_barrier.png";

    public GuardBarrier() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

         int finalBlock = DynamicVars.Block.IntValue;
        // 恢复 2 生命
        await CreatureCmd.Heal(
            creature,DynamicVars.Heal.IntValue
        );       
        // 芒属性：格挡 +2
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            finalBlock += 2;
            // 获得格挡
            await CreatureCmd.GainBlock(
            creature,
            finalBlock,
            DynamicVars.Block.Props,
            cardPlay
            );

            // 需气氛 ≥6 才能发动
            int currentAtmosphere = creature.GetPowerAmount<AtmospherePower>();
            if (currentAtmosphere < 6)
            {
                return;
            }

        // 消耗 6 气氛
            await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, -6, creature, this);

        // 获得 2 点卡牌能量
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        }  
        await CreatureCmd.GainBlock(
            creature,
            finalBlock,
            DynamicVars.Block.Props,
            cardPlay
        );
     
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡 +3
        DynamicVars.Block.UpgradeValueBy(3);
    }

  
        // "获得 {Block} 点格挡，恢复 {Heal} 点生命。\n" +
        // "芒属性时：格挡+2。\n" +
        // "需气氛≥6，打出时消耗6气氛，获得2点能量。\n" +
        // "升级：格挡+3。";
}