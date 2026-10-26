using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class VerdictStrike : CustomCardModel
{
    private const int energyCost = 2; // 基础2费
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/verdict_strike.png";

    public VerdictStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        if (cardPlay.Target == null) return;
        Creature player = Owner.Creature;

        // 造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        
        // 获取当前资源
        int elemental = player.GetPowerAmount<ElementEnergyPower>();
        int atmosphere = player.GetPowerAmount<AtmospherePower>();
       
        // 资源不足，无法打出
        if (elemental < 8 || atmosphere < 4)
            return;

         // 荒属性：沙龙本回合伤害 +2
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,player, 2, player, this);
            if (buff != null)
            {
                buff.Duration = 1; // 只持续当前回合！
            }
        }
        
        bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
            // 消耗资源
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -8, player, this);
        }
        await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -4, player, this);

        
    }

    protected override void OnUpgrade()
    {
        // 升级效果：费用从 2 → 1
        base.EnergyCost.UpgradeBy(-1);
    }
}