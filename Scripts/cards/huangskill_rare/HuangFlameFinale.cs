using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class HuangFlameFinale : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_flame_finale.png";

    public HuangFlameFinale() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;

        // 每1名沙龙 = 获得1点能量
        int salonCount = player.GetPowerAmount<ShadowSalonPower>();
        if (salonCount > 0)
        {
            await PlayerCmd.GainEnergy(salonCount, player.Player);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：1费 → 0费
        base.EnergyCost.UpgradeBy(-1);
    }

    //4.荒焰终章：耗1点卡牌能量，每1沙龙→1卡牌能量，升级0能量
}