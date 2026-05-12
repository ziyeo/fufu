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
public class AtmosphereSurge : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/atmosphere_surge.png";

    public AtmosphereSurge() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        // 基础效果：获得 10 气氛
        await PowerCmd.Apply<AtmospherePower>(choiceContext,player, 10, player, this);

        // 荒属性：解锁 5气氛 → 1点能量 的被动效果
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int atmosphere = player.GetPowerAmount<AtmospherePower>();
            int energyGain = atmosphere / 5;

            if (energyGain > 0)
            {
            // 扣除气氛
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -energyGain * 5, player, this);
            // 获得能量
            await PlayerCmd.GainEnergy(energyGain,player.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：2费 → 1费
        base.EnergyCost.UpgradeBy(-1);
    }
}