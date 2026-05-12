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
using System.Linq;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class DesolateJudgment : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15, ValueProp.Move)];

    public override string PortraitPath => "res://fufu/images/card_portraits/desolate_judgment.png";

    public DesolateJudgment() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        if (cardPlay.Target is null) return;

        Creature player = Owner.Creature;
        decimal finalDamage = DynamicVars.Damage.BaseValue;

        // 荒属性效果
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int ele = player.GetPowerAmount<ElementEnergyPower>();
            int atm = player.GetPowerAmount<AtmospherePower>();

            if (ele >= 6 && atm >= 3)
            {
                // 扣除资源
                bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
                // 没有掌控 → 正常扣费
                if (!hasElementMastery)
                {
                await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -6, player, this);
                }
                await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -3, player, this);

                // 本场战斗每失去1点生命 → 伤害 +4
                decimal lostHp = player.MaxHp - player.CurrentHp;
                finalDamage += lostHp * 4;
            }
        }

        // 造成最终伤害
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}