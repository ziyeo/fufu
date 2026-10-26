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
public class WaterStrike : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/water_strike.png";

    public WaterStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        if (cardPlay.Target == null) return;
        Creature player = Owner.Creature;

        // 造成水属性伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // ============= 荒属性：沙龙伤害+1 =============
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,player, 1, player, this);
            if (buff != null)
            {
                buff.Duration = 1; // 只持续当前回合！
            }
        }

        // 命中获得1元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, 1, player, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}