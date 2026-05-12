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
public class SalonBoostUp : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override string PortraitPath => "res://fufu/images/card_portraits/salon_boost_up.png";

    public SalonBoostUp() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        // 基础效果：沙龙伤害 +2（1回合）
        int damageBoost = 2;

        // 荒属性：提升为 +3，并且获得 2 气氛
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            damageBoost = 3;
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, 2, player, this);
        }

        // 施加临时增伤 BUFF，持续 1 回合
        var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,player, damageBoost, player, this);
        if (buff != null)
        {
            buff.Duration = 1;
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：2费 → 1费
        base.EnergyCost.UpgradeBy(-1);
    }
    //3.耗 2 点卡牌能量，沙龙伤害 + 2（1 回合）；角色荒属性时 + 3 伤害，获 2 气氛。升级1能量。
}