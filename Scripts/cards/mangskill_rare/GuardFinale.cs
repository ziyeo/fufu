using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class GuardFinale : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(25,ValueProp.Move),
        new HealVar(10)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/guard_finale.png";

    public GuardFinale() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;

        // 基础效果：25格挡 + 15回血
        await CreatureCmd.GainBlock(
            player, 
            DynamicVars.Block.BaseValue, 
            DynamicVars.Block.Props, 
            cardPlay);
        await CreatureCmd.Heal(player, DynamicVars.Heal.BaseValue);

        // 芒属性判定
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int elem = player.GetPowerAmount<ElementEnergyPower>();
            int atmos = player.GetPowerAmount<AtmospherePower>();

            // 条件：元素≥7 且 气氛≥2
            if (elem >= 7 && atmos >= 2)
            {
                // 扣除资源
                bool hasElementMastery = player.HasPower<ElementMasteryPower>();
                if (!hasElementMastery)
                {
                    await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -7, player, this);
                }
                await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -2, player, this);

                // 抽2牌
                await CardPileCmd.Draw(choiceContext, 2, player.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：3费 → 2费
        base.EnergyCost.UpgradeBy(-1);
    }

    //4.守护终章：耗3费，获25格挡+恢复15血；芒属性时需元素≥7+气氛≥2，打出扣7元素+2气氛，抽2牌。升级2能量
}