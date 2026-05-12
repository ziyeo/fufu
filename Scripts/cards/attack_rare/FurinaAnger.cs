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
public class FurinaAnger : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/furina_anger.png";

    public FurinaAnger() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        if (cardPlay.Target == null) return;

        // 基础伤害
        int finalDamage = DynamicVars.Damage.IntValue;

        // 荒属性 +5 伤害
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            finalDamage += 5;
        }

        // 每1个召唤物 +1伤害
        int summonCount = creature.GetPowerAmount<ShadowSalonPower>();
        finalDamage += summonCount;

        // 每5点元素能量 +1伤害
        int element = creature.GetPowerAmount<ElementEnergyPower>();
        finalDamage += element / 5;

        // 造成最终伤害
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级 +5 伤害
        DynamicVars.Damage.UpgradeValueBy(5);
    }

    //1.芙宁娜之怒：耗 2 点卡牌能量，造 13 普通 ，角色荒属性时伤害 + 5 ，每有1召唤物伤害+1，每有5元素能量，伤害+1，升级+5伤害
}