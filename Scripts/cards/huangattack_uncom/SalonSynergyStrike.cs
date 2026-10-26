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
public class SalonSynergyStrike : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10,ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/salon_synergy_strike.png";

    public SalonSynergyStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        if (cardPlay.Target is null) return;

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;

        // 基础伤害
        int damage = DynamicVars.Damage.IntValue;

        // 荒属性时：每1个沙龙 +2伤害
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int salonCount = creature.GetPowerAmount<ShadowSalonPower>();
            damage += salonCount * 2;
        }

        // 造成伤害
        await DamageCmd.Attack(damage)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
    //2.耗 2 点卡牌能量，造 10伤害 ，角色荒属性时每有 1 名沙龙伤害 + 2；升级造13伤害。
}