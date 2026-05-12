using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class SalonBoost : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 基础格挡：每1个沙龙 = 2点
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
       new BlockVar(2, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/salon_boost.png";

    public SalonBoost() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        // 获取当前沙龙数量
        int salonCount = player.GetPowerAmount<ShadowSalonPower>();

        // 计算总格挡 = 沙龙数 × 基础值
        int totalBlock = salonCount * DynamicVars.Block.IntValue;

        // 获得格挡
        if (totalBlock > 0)
        {
            await CreatureCmd.GainBlock(
            player, 
            totalBlock,
            DynamicVars.Block.Props, 
            cardPlay
        );

        }
    }

    protected override void OnUpgrade()
    {
        // 升级：每沙龙格挡 +1（2→3）
        DynamicVars.Block.UpgradeValueBy(1);
    }
}