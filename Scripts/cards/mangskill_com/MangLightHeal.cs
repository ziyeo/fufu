using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using Fufu.FufuModCode.Character;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class MangLightHeal : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new HealVar(3)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_light_heal.png";

    public MangLightHeal() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        Player ownerPlayer = player.Player;
        int healAmount = DynamicVars.Heal.IntValue;

        // 芒属性 + 气氛 ≥5 → 治疗额外+3
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int currentAtm = player.GetPowerAmount<AtmospherePower>();
            if (currentAtm >= 5)
            {
                healAmount += 3;
            }
        }

        // 恢复生命
        await CreatureCmd.Heal(player, healAmount);

        // 资源互转：4 元素 → 1 点卡牌能量
        int currentElement = player.GetPowerAmount<ElementEnergyPower>();
        if (currentElement >= 4)
        {
            bool hasMastery = player.HasPower<ElementMasteryPower>();
            int energyToGain = currentElement/4; // 每4点元素能量转换为1点卡牌能量
            // 无元素掌控才扣元素
            if (!hasMastery)
            {
                await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -(energyToGain*4), player, this);
            }

            // 获得1点卡牌能量
            await PlayerCmd.GainEnergy(energyToGain, ownerPlayer);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：治疗+2
        DynamicVars.Heal.UpgradeValueBy(2);
    }

   
        // "恢复 {Heal} 点生命。\n" +
        // "芒属性时：若气氛≥5，恢复额外+3。\n" +
        // "资源互转：4元素→1卡牌能量。\n" +
        // "消耗。\n" +
        // "升级：生命恢复+2。";
}