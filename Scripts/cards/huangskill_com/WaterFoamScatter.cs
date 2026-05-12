using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class WaterFoamScatter : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(1m)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/water_foam_scatter.png";

    public WaterFoamScatter() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        if (cardPlay.Target == null) return;

        // 检查气氛 ≥5
        int atmosphere = player.GetPowerAmount<AtmospherePower>();
        if (atmosphere < 5) return;

        // 扣除 5 气氛
        await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -5, player, this);

        // 施加易伤
        await PowerCmd.Apply<VulnerablePower>(choiceContext,cardPlay.Target!, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
    //4.水沫散射：耗 1 点卡牌能量，需气氛≥5，打出扣 5 气氛，敌人获 1 层易伤。升级2层易伤。
}