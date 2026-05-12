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
public class HuangDestroyStrike : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    // 标记为消耗牌
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_destroy_strike.png";

    public HuangDestroyStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;
        if (cardPlay.Target == null||CombatState==null) return;
        // 基础伤害
        int finalDamage = DynamicVars.Damage.IntValue;

        // 荒属性：每4气氛 +1伤害
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int atmosphere = creature.GetPowerAmount<AtmospherePower>();
            finalDamage += atmosphere / 4;
        }
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
          DynamicVars.Damage.UpgradeValueBy(5);
    }

    //2.荒性灭击：（消耗牌）耗 2 点卡牌能量，对敌人造 12 普通伤害；荒属性时，每有4气氛值伤害+1。升级+5伤害。
}