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
public class SingerApotheosis : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/singer_apotheosis.png";

    public SingerApotheosis() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) 
            return;

        // 仅芒属性生效
        if (FufuModCode.Character.FufuMod.CurrentAttr != FufuAttr.Mang)
            return;

        // 施加永久治疗加成
        var power = await PowerCmd.Apply<SingerHealBuffPower>(choiceContext,creature, 4, creature, this);
        if (power != null)
        {
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：能量消耗 -1
        base.EnergyCost.UpgradeBy(-1);
    }
}