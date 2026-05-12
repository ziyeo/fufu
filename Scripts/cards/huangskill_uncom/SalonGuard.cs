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
public class SalonGuard : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 基础格挡：每1个沙龙 = 3点
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move)];

    public override string PortraitPath => "res://fufu/images/card_portraits/salon_guard.png";

    public SalonGuard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;
        
        // 每有1名沙龙，获得对应格挡
        int salonCount = player.GetPowerAmount<ShadowSalonPower>();
        int totalBlock = salonCount * DynamicVars.Block.IntValue;
        if (totalBlock > 0)
        {
            await CreatureCmd.GainBlock(
            player, 
            totalBlock,
            DynamicVars.Block.Props, 
            cardPlay
            );
        }

        // 荒属性：沙龙水伤 +2（1回合）
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int currentAtmo = player.GetPowerAmount<AtmospherePower>();
            if (currentAtmo < 8) 
            return;
            // 扣除 8 气氛
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -8, player, this);

            var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,player, 2, player, this);
            if (buff != null)
            {
                buff.Duration = 1;
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：每沙龙格挡 +2（3→5）
        DynamicVars.Block.UpgradeValueBy(2);
    }
}