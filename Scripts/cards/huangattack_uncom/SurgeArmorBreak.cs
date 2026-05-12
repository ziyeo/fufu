using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class SurgeArmorBreak : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/surge_armor_break.png";

    public SurgeArmorBreak() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        if (cardPlay.Target == null) return;

        // 造成 9 点普通伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 荒属性：沙龙本次伤害 +2（1回合临时增伤）
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,player, 2, player, this);
            if (buff != null)
            {
                buff.Duration = 1;
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +3 → 9 → 12
        DynamicVars.Damage.UpgradeValueBy(3);
    }
    //浪涌破甲：耗 2 点卡牌能量，造 9 普通 ；角色荒属性时沙龙本次伤害 + 2；升级+3伤害
}